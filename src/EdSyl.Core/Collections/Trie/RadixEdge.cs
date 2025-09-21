using System.Collections;
using static EdSyl.Collections.ThrowHelper;

namespace EdSyl.Collections;

public sealed partial class RadixEdge : IRadixEdge
{
    private static readonly List<RadixEdge> Empty = [];

    private bool leaf;
    private string label;
    private List<RadixEdge>? edges;

    /// <inheritdoc />
    public bool Leaf => leaf;

    /// <inheritdoc />
    public ReadOnlySpan<char> Label => label;

    /// <inheritdoc />
    public int Count => edges?.Count ?? 0;

    /// <inheritdoc />
    public IRadixEdge this[int index] => (edges ?? Empty)[index];

    public RadixEdge()
    {
        label = string.Empty;
    }

    public RadixEdge(ReadOnlySpan<char> label)
    {
        leaf = true;
        this.label = label.IsEmpty ? string.Empty : new(label);
    }

    private RadixEdge(RadixEdge other)
    {
        leaf = other.leaf;
        label = other.label;
        edges = other.edges;
    }

    /// <inheritdoc />
    public override string ToString() => label;

    /// <summary> Returns an enumerator that iterates through the collection. </summary>
    public List<RadixEdge>.Enumerator GetEnumerator() => (edges ?? Empty).GetEnumerator();

    /// <inheritdoc />
    IEnumerator<IRadixEdge> IEnumerable<IRadixEdge>.GetEnumerator() => (edges ?? Empty).GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => (edges ?? Empty).GetEnumerator();
}

[SuppressMessage("Design", "CA1000", Justification = "Private Access")]
public partial class RadixEdge
{
    internal static bool Insert(List<RadixEdge> edges, ReadOnlySpan<char> key, IComparer<char>? comparer, InsertionBehavior behaviour)
    {
        while (key.Length > 0)
        {
            // binary search for common prefix
            var index = edges.BinarySearch(key, comparer, out var length, exact: false);

            // insert if no common prefix
            RadixEdge edge;
            if (index < 0)
            {
                edge = new(key);
                edges.Insert(~index, edge);
                return true;
            }

            edge = edges[index];
            key = key[length..];

            // split into two leafs with common prefix
            if (length < edge.label.Length)
            {
                Split(edge, length, key, comparer);
                return true;
            }

            // exact match
            if (key.IsEmpty)
            {
                // already exists?
                if (edge.leaf)
                    return behaviour switch
                    {
                        InsertionBehavior.ThrowOnExisting => throw AddingDuplicateWithKeyArgumentException(key.ToString(), nameof(key)),
                        _ => false,
                    };

                edge.leaf = true;
                return true;
            }

            // advance
            edges = edge.edges ??= [];
        }

        return false;
    }

    private static RadixEdge Split(RadixEdge stem, int length, ReadOnlySpan<char> key, IComparer<char>? comparer)
    {
        var copy = new RadixEdge(stem);
        (stem.label, copy.label) = stem.label.Split(length);

        if (key.IsEmpty)
        {
            stem.edges = [copy];
            stem.leaf = true;
            return stem;
        }

        var suffix = new RadixEdge(key);
        stem.edges = ToList(copy, suffix, comparer);
        stem.leaf = false;
        return suffix;
    }

    private static List<RadixEdge> ToList(RadixEdge x, RadixEdge? y, IComparer<char>? comparer)
    {
        if (y == null) return [x];
        return x.label[0].Compare(y.label[0], comparer) < 0
            ? [x, y]
            : [y, x];
    }
}
