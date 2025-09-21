using System.Buffers;

namespace EdSyl;

/// <summary>
/// A temporary contract renting an array from the <see cref="Dispose" />.
/// Array will be returned to the pool upon <see cref="Dispose" />.
/// </summary>
/// <typeparam name="T">Type of array elements.</typeparam>
public ref struct ArrayRent<T>
{
    private T[] array;

    public ArrayRent(int size, out T[] buffer)
    {
        buffer = array = size > 0
            ? ArrayPool<T>.Shared.Rent(size)
            : [];
    }

    public ArrayRent(ReadOnlySpan<T> source, out T[] buffer)
        : this(source.Length, out buffer)
    {
        if (array.Length > 0)
            source.CopyTo(array);
    }

    public ArrayRent(ICollection<T> source, out T[] buffer, out int size)
        : this(size = source.Count, out buffer)
    {
        if (size > 0)
            source.CopyTo(array, 0);
    }

    public ArrayRent(IReadOnlyCollection<T> source, out T[] buffer, out int size)
        : this(size = source.Count, out buffer)
    {
        var index = 0;
        foreach (var item in source)
            buffer[index++] = item;
    }

    public void Dispose()
    {
        array = Arrays.Return(array, true);
    }
}
