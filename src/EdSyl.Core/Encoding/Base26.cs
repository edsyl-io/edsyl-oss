namespace EdSyl.Encoding;

public static class Base26
{
    private const int Basis = 26;
    private const int UInt32Digits = 7;
    private const int UInt64Digits = 14;

    public static string Encode(int value)
    {
        return Encode((uint)value);
    }

    public static string? Encode(int? value)
    {
        return value.HasValue
            ? Encode((uint)value.Value)
            : null;
    }

    public static string Encode(uint value)
    {
        return Encode(value, stackalloc char[UInt32Digits]).ToString();
    }

    public static string? Encode(uint? value)
    {
        return value.HasValue
            ? Encode(value.Value, stackalloc char[UInt32Digits]).ToString()
            : null;
    }

    public static string Encode(ulong value)
    {
        return Encode(value, stackalloc char[UInt64Digits]).ToString();
    }

    public static string? Encode(ulong? value)
    {
        return value.HasValue
            ? Encode(value.Value, stackalloc char[UInt64Digits]).ToString()
            : null;
    }

    public static int DecodeInt32(ReadOnlySpan<char> chars)
    {
        return (int)DecodeUInt32(chars);
    }

    public static ulong DecodeUInt32(ReadOnlySpan<char> chars)
    {
        uint value = 0;
        uint multiplier = 1;

        for (var i = chars.Length - 1; i >= 0; i--)
        {
            value += ValueOf(chars[i]) * multiplier;
            multiplier *= Basis;
        }

        return value;
    }

    public static ulong DecodeUInt64(ReadOnlySpan<char> chars)
    {
        ulong value = 0;
        ulong multiplier = 1;

        for (var i = chars.Length - 1; i >= 0; i--)
        {
            value += ValueOf(chars[i]) * multiplier;
            multiplier *= Basis;
        }

        return value;
    }

    private static Span<char> Encode(uint value, Span<char> chars)
    {
        var index = chars.Length;
        do
        {
            chars[--index] = CharOf(value % Basis);
            value /= Basis;
        } while (value > 0);

        return chars[index..];
    }

    private static Span<char> Encode(ulong value, Span<char> chars)
    {
        var index = chars.Length;
        do
        {
            chars[--index] = CharOf(value % Basis);
            value /= Basis;
        } while (value > 0);

        return chars[index..];
    }

    private static uint ValueOf(char c) => c switch
    {
        'A' => 0,
        'B' => 1,
        'C' => 2,
        'D' => 3,
        'E' => 4,
        'F' => 5,
        'G' => 6,
        'H' => 7,
        'I' => 8,
        'J' => 9,
        'K' => 10,
        'L' => 11,
        'M' => 12,
        'N' => 13,
        'O' => 14,
        'P' => 15,
        'Q' => 16,
        'R' => 17,
        'S' => 18,
        'T' => 19,
        'U' => 20,
        'V' => 21,
        'W' => 22,
        'X' => 23,
        'Y' => 24,
        'Z' => 25,
        _ => throw new ArgumentOutOfRangeException($"'{c}' is not a valid base32 character"),
    };

    private static char CharOf(ulong v) => v switch
    {
        0 => 'A',
        1 => 'B',
        2 => 'C',
        3 => 'D',
        4 => 'E',
        5 => 'F',
        6 => 'G',
        7 => 'H',
        8 => 'I',
        9 => 'J',
        10 => 'K',
        11 => 'L',
        12 => 'M',
        13 => 'N',
        14 => 'O',
        15 => 'P',
        16 => 'Q',
        17 => 'R',
        18 => 'S',
        19 => 'T',
        20 => 'U',
        21 => 'V',
        22 => 'W',
        23 => 'X',
        24 => 'Y',
        25 => 'Z',
        _ => throw new ArgumentOutOfRangeException($"'{v}' is not a valid base32 value"),
    };
}
