using System.Collections;

namespace EdSyl.Linq;

public static partial class Extensions
{
    /// <summary> Determines whether a sequence contains any elements. </summary>
    /// <param name="source">Collection to check.</param>
    public static bool Any(this IEnumerable source)
    {
        if (source is ICollection collection)
            return collection.Count > 0;

        var iterator = source.GetEnumerator();
        using (iterator as IDisposable)
            return iterator.MoveNext();
    }

    /// <summary> Try to non-empty iterator for the given collection of items. </summary>
    /// <param name="items">Collection of items to get iterator for.</param>
    /// <param name="iterator">A non-empty iterator advanced to the first element.</param>
    /// <typeparam name="T">Type of elements in collection.</typeparam>
    /// <returns>True when provided collection is not empty; false otherwise.</returns>
    public static bool TryMoveNext<T>(this IEnumerable<T>? items, [MaybeNullWhen(false)] out IEnumerator<T> iterator)
    {
        if (items != null)
        {
            iterator = items.GetEnumerator();
            if (iterator.MoveNext()) return true;
            iterator.Dispose();
            iterator = null;
            return false;
        }

        iterator = null;
        return false;
    }

    public static IList<T> AsList<T>(this IEnumerable<T> source)
        => source as IList<T> ?? source.ToList();

    public static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
        => source as ICollection<T> ?? source.ToList();

    public static bool HasMany<T>(this IEnumerable<T> items)
    {
        if (items.TryGetNonEnumeratedCount(out var count))
            return count > 1;

        using var it = items.GetEnumerator();
        return it.MoveNext() && it.MoveNext();
    }
}
