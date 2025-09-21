namespace EdSyl.Collections;

public interface IRadixEdge : IReadOnlyList<IRadixEdge>
{
    /// <summary> Whether current edge represents a leaf node. </summary>
    bool Leaf { get; }

    /// <summary> Part of the key associated with the node. </summary>
    ReadOnlySpan<char> Label { get; }
}

public interface IRadixEdge<T> : IRadixEdge, IReadOnlyList<IRadixEdge<T>>
{
    bool TryGetValue([MaybeNullWhen(false)] out T value);
}
