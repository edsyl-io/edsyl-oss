using System.Buffers;
using System.Text;

namespace EdSyl.Buffers;

/// <summary>
/// A hybrid buffer that efficiently manages character data using both stack and heap memory.
/// Uses a provided stack buffer and automatically switches to a pooled array for larger content.
/// </summary>
public ref struct StringBuffer : IBufferWriter<char>
{
    private int length;
    private char[]? heap;
    private Span<char> buffer;

    public StringBuffer() => buffer = default;
    public StringBuffer(int size) => buffer = heap = ArrayPool<char>.Shared.Rent(size);
    public StringBuffer(Span<char> buffer) => this.buffer = buffer;

    /// <summary> Length of the current buffer. </summary>
    public int Length => length;

    /// <inheritdoc />
    public void Advance(int count) => length += count;

    /// <inheritdoc />
    public Memory<char> GetMemory(int sizeHint = 0)
    {
        // sanity checks
        if (sizeHint == 0)
            return new([]);

        // allocate heap
        var size = length + sizeHint;
        if (heap == null || size > heap.Length)
            MemoryCopy(size);

        return heap.AsMemory(length);
    }

    /// <inheritdoc />
    public Span<char> GetSpan(int sizeHint = 0)
    {
        GrowBy(sizeHint);
        return buffer[length..];
    }

    /// <summary> Write the string representation of a specified char. </summary>
    /// <param name="value">UTF-16-encoded code unit to append.</param>
    /// <seealso cref="StringBuilder.Append(char)" />
    public int Write(char value)
    {
        GrowBy(1);
        buffer[length++] = value;
        return 1;
    }

    /// <summary> Write the string representation of a specified 32-bit signed integer. </summary>
    /// <param name="value">Value to write.</param>
    /// <seealso cref="StringBuilder.Append(int)" />
    public int Write(int value) => Format(value, 11);

    /// <summary> Write the string representation of a specified 64-bit signed integer. </summary>
    /// <param name="value">Value to write.</param>
    /// <seealso cref="StringBuilder.Append(long)" />
    public int Write(long value) => Format(value, 20);

    /// <summary> Write the string representation of a specified 64-bit unsigned integer. </summary>
    /// <param name="value">Value to write.</param>
    /// <seealso cref="StringBuilder.Append(ulong)" />
    public int Write(ulong value) => Format(value, 20);

    /// <summary> Write the string representation of a specified read-only character span. </summary>
    /// <param name="value">Value to write.</param>
    /// <seealso cref="StringBuilder.Append(ReadOnlySpan{char})" />
    public int Write(ReadOnlySpan<char> value)
    {
        GrowBy(value.Length);
        value.CopyTo(buffer[length..]);
        length += value.Length;
        return value.Length;
    }

    /// <summary> Get a span over the content of this buffer. </summary>
    public readonly ReadOnlySpan<char> AsSpan()
    {
        return length != 0
            ? buffer[..length]
            : default;
    }

    /// <inheritdoc />
    public readonly override string? ToString()
    {
        return length != 0
            ? buffer[..length].ToString()
            : null;
    }

    /// <inheritdoc cref="ToString()" />
    public string? ToString(bool dispose)
    {
        // TODO: automatic dispose when .ToString
        var text = ToString();
        if (dispose) Dispose();
        return text;
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    public void Dispose()
    {
        length = 0;
        if (heap == null || heap.Length == 0) return;
        ArrayPool<char>.Shared.Return(heap);
        buffer = heap = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Format<T>(T value, int size) where T : ISpanFormattable
    {
        GrowBy(size);
        return !value.TryFormat(buffer[length..], out var written, default, default)
            ? Write(value.ToString())
            : written;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int count)
    {
        var size = length + count;
        if (size > buffer.Length)
            MemoryCopy(size);
    }

    private void MemoryCopy(int size)
    {
        var array = ArrayPool<char>.Shared.Rent(size);
        if (length > 0) buffer[..length].CopyTo(array);
        if (heap is { Length: > 0 }) ArrayPool<char>.Shared.Return(heap);
        buffer = heap = array;
    }
}

public static class StringBuffers
{
    /// <summary> Compute the size necessary to write a list of provided items. </summary>
    /// <param name="items">List of items necessary to write.</param>
    /// <param name="gap">Number of characters between each item.</param>
    public static int SizeOf(ReadOnlySpan<string?> items, int gap = 0)
    {
        switch (items.Length)
        {
            case 0: return 0;
            case 1: return items[0]?.Length ?? 0;
            default: return SizeOfList(items, gap);
        }
    }

    private static int SizeOfList(ReadOnlySpan<string?> items, int gap = 0)
    {
        var size = 0;
        foreach (ReadOnlySpan<char> item in items)
            if (item.Length > 0)
                size += item.Length + gap;

        return size - gap;
    }
}
