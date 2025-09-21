using System.Runtime.InteropServices;

namespace EdSyl;

public static class MarshalExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "MA0016", Justification = "Unsafe")]
    public static Span<T> AsSpan<T>(this List<T>? list) => CollectionsMarshal.AsSpan(list);

    [SuppressMessage("Design", "MA0016", Justification = "Unsafe")]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(this List<T>? list) => CollectionsMarshal.AsSpan(list);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "MA0016", Justification = "Unsafe")]
    public static ref T ElementRef<T>(this List<T> list, int index) => ref CollectionsMarshal.AsSpan(list)[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [SuppressMessage("Design", "MA0016", Justification = "Unsafe")]
    public static ref T Insert<T>(this List<T> list, int index)
    {
        list.Insert(index, default!);
        return ref list.ElementRef(index);
    }
}
