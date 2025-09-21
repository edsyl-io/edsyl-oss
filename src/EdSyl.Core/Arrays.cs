using System.Buffers;

namespace EdSyl;

/// <summary>
/// Provides utility methods for <see cref="Array" />.
/// </summary>
public static class Arrays
{
    /// <summary>Determines whether the array contains a specific value.</summary>
    /// <param name="array">Array to check for a value.</param>
    /// <param name="item">The object to locate in the array.</param>
    /// <typeparam name="T">Type of the elements within the array.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains<T>(this T[] array, T item)
        => Array.IndexOf(array, item) >= 0;

    /// <summary>Rent an array of the given size. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="size">Minimum size of the array.</param>
    /// <param name="buffer">Field to hold the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(int size, out T[] buffer)
    {
        return new(size, out buffer);
    }

    /// <summary> Rent an array filled with items from the given source. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="source">Source array to copy.</param>
    /// <param name="array">Field to hold the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(ReadOnlySpan<T> source, out T[] array)
    {
        return new(source, out array);
    }

    /// <summary> Rent an array filled with items from the given source. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="source">Source collection to copy.</param>
    /// <param name="span">Field to hold the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(ICollection<T> source, out ReadOnlySpan<T> span)
    {
        var rent = new ArrayRent<T>(source, out var buffer, out var size);
        span = buffer.AsSpan(0, size);
        return rent;
    }

    /// <summary> Rent an array filled with items from the given source. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="source">Source collection to copy.</param>
    /// <param name="buffer">Field to hold the rented array.</param>
    /// <param name="size">Number of elements copied to the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(ICollection<T> source, out T[] buffer, out int size)
    {
        return new(source, out buffer, out size);
    }

    /// <summary> Rent an array filled with items from the given source. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="source">Source collection to copy.</param>
    /// <param name="span">Field to hold the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(IReadOnlyCollection<T> source, out ReadOnlySpan<T> span)
    {
        var rent = new ArrayRent<T>(source, out var buffer, out var size);
        span = buffer.AsSpan(0, size);
        return rent;
    }

    /// <summary> Rent an array filled with items from the given source. </summary>
    /// <typeparam name="T">Type of array elements.</typeparam>
    /// <param name="source">Source collection to copy.</param>
    /// <param name="buffer">Field to hold the rented array.</param>
    /// <param name="size">Number of elements copied to the rented array.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayRent<T> Rent<T>(IReadOnlyCollection<T> source, out T[] buffer, out int size)
    {
        return new(source, out buffer, out size);
    }

    /// <summary>Sets a range of elements in an array to the default value of each element type.</summary>
    /// <param name="array">The array whose elements need to be cleared.</param>
    /// <param name="length">The number of elements to clear.</param>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(T[] array, int length)
    {
        if (length > 0)
            Array.Clear(array, 0, length);
    }

    /// <summary>Return the array back to the <see cref="ArrayPool{T}" />.</summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to return back to the <see cref="ArrayPool{T}" />.</param>
    /// <param name="clear">Whether the contents of the buffer should be cleared before reuse.</param>
    /// <returns>An empty array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Return<T>(T[] array, bool clear = false)
    {
        if (array.Length > 0)
            ArrayPool<T>.Shared.Return(array, clear);

        return [];
    }

    /// <summary>Return the array to the <see cref="ArrayPool{T}" />.</summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to return to the <see cref="ArrayPool{T}" />.</param>
    /// <param name="length">Number of the elements to clear from the array.</param>
    /// <returns>An empty array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Return<T>(T[] array, int length)
    {
        if (length > 0) Array.Clear(array, 0, length);
        if (array.Length > 0) ArrayPool<T>.Shared.Return(array);
        return [];
    }

    /// <summary>Inserts an item to the array at the specified index.</summary>
    /// <param name="array">The array where elements need to be inserted.</param>
    /// <param name="length">Number of the elements to clear from the array.</param>
    /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
    /// <param name="item">The object to insert into the array.</param>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    public static void Insert<T>(ref T[] array, ref int length, int index, T item)
    {
        if (array.Length > length)
        {
            if (index < length)
                Array.Copy(array, index, array, index + 1, length - index);
        }
        else
        {
            var next = new T[array.Length == 0 ? 4 : 2 * length];
            Array.Copy(array, 0, next, 0, index);

            if (index < length)
                Array.Copy(array, index, next, index + 1, length - index);

            if (length > 0)
                Array.Clear(array, 0, length);

            array = next;
        }

        array[index] = item;
        length++;
    }

    /// <summary> Grow using length doubling strategy until it has enough space to fit provided capacity. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if needed.</param>
    /// <param name="minimumLength">Minimum capacity of the array needed.</param>
    /// <param name="length">Number of elements in use by the <paramref name="array" />.</param>
    /// <returns>An array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Grow<T>(T[] array, int minimumLength, int length)
    {
        if (array.Length >= minimumLength)
            return array;

        var next = new T[GrowLength(array, minimumLength)];
        if (length > 0) Array.Copy(array, 0, next, 0, length);
        return next;
    }

    /// <summary> Grow using length doubling strategy until it has enough space to fit provided capacity. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if needed.</param>
    /// <param name="minimumLength">Minimum capacity of the array needed.</param>
    /// <param name="length">Number of elements in use by the <paramref name="array" />.</param>
    /// <returns>True when a new array has been allocated; false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Grow<T>(ref T[] array, int minimumLength, int length)
    {
        var prev = array;
        array = Grow(array, minimumLength, length);
        return prev != array;
    }

    /// <summary> Grow the array acquired from the <see cref="ArrayPool{T}" /> if required. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if required.</param>
    /// <param name="minimumLength">The minimum length of the array.</param>
    /// <returns>An array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GrowPool<T>(T[] array, int minimumLength)
    {
        return array.Length < minimumLength
            ? ResizePool(array, GrowLength(array, minimumLength))
            : array;
    }

    /// <summary> Grow the array acquired from the <see cref="ArrayPool{T}" /> if required. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if required.</param>
    /// <param name="minimumLength">The minimum length of the array.</param>
    /// <returns>True when a new array has been allocated; false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GrowPool<T>(ref T[] array, int minimumLength)
    {
        if (array.Length >= minimumLength) return false;
        array = ResizePool(array, GrowLength(array, minimumLength));
        return true;
    }

    /// <summary> Grow the array acquired from the <see cref="ArrayPool{T}" /> if required. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if required.</param>
    /// <param name="minimumLength">Minimum capacity of the array needed.</param>
    /// <param name="length">Number of elements used by the <paramref name="array" /> array.</param>
    /// <returns>An array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GrowPool<T>(T[] array, int minimumLength, int length)
    {
        return array.Length < minimumLength
            ? ResizePool(array, GrowLength(array, minimumLength), length)
            : array;
    }

    /// <summary> Grow the array acquired from the <see cref="ArrayPool{T}" /> if required. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to grow if required.</param>
    /// <param name="minimumLength">Minimum capacity of the array needed.</param>
    /// <param name="length">Number of elements used by the <paramref name="array" /> array.</param>
    /// <returns>An array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GrowPool<T>(ref T[] array, int minimumLength, int length)
    {
        if (array.Length >= minimumLength) return false;
        array = ResizePool(array, GrowLength(array, minimumLength), length);
        return true;
    }

    /// <summary> Exchange array to array of reduced size if length is less than half of the array length. </summary>
    /// <typeparam name="T">Type of elements in the array.</typeparam>
    /// <param name="array">Array to shrink if necessary.</param>
    /// <param name="length">Number of elements used by the <paramref name="array" /> array.</param>
    /// <returns>An array to use in place of the existing array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] HalvePool<T>(T[] array, int length)
    {
        return array.Length >= length * 2
            ? ResizePool(array, length, length)
            : array;
    }

    /// <summary> Get the next power of two. </summary>
    /// <param name="v">Value to get scale up to power of two.</param>
    /// <remarks>https://graphics.stanford.edu/~seander/bithacks.html#RoundUpPowerOf2</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int NextPow2(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        v++;
        return v;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GrowLength<T>(T[] array, int minimumLength)
    {
        return Math.Max(array.Length * 2, minimumLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T[] ResizePool<T>(T[] array, int minimumLength)
    {
        Return(array, true);
        return minimumLength > 0
            ? ArrayPool<T>.Shared.Rent(minimumLength)
            : [];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T[] ResizePool<T>(T[] array, int minimumLength, int length)
    {
        var next = minimumLength > 0
            ? ArrayPool<T>.Shared.Rent(minimumLength)
            : [];

        if (length > 0)
            Array.Copy(array, 0, next, 0, length);

        Return(array);
        return next;
    }
}
