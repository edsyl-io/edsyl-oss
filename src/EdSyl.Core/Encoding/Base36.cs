namespace EdSyl.Encoding;

public static class Base36
{
    private const int Basis = 36;
    private const int UInt32Digits = 7;
    private const int UInt64Digits = 13;

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
        '0' => 0,
        '1' => 1,
        '2' => 2,
        '3' => 3,
        '4' => 4,
        '5' => 5,
        '6' => 6,
        '7' => 7,
        '8' => 8,
        '9' => 9,
        'A' => 10,
        'B' => 11,
        'C' => 12,
        'D' => 13,
        'E' => 14,
        'F' => 15,
        'G' => 16,
        'H' => 17,
        'I' => 18,
        'J' => 19,
        'K' => 20,
        'L' => 21,
        'M' => 22,
        'N' => 23,
        'O' => 24,
        'P' => 25,
        'Q' => 26,
        'R' => 27,
        'S' => 28,
        'T' => 29,
        'U' => 30,
        'V' => 31,
        'W' => 32,
        'X' => 33,
        'Y' => 34,
        'Z' => 35,
        _ => throw new ArgumentOutOfRangeException($"'{c}' is not a valid base32 character"),
    };

    private static char CharOf(ulong v) => v switch
    {
        0 => '0',
        1 => '1',
        2 => '2',
        3 => '3',
        4 => '4',
        5 => '5',
        6 => '6',
        7 => '7',
        8 => '8',
        9 => '9',
        10 => 'A',
        11 => 'B',
        12 => 'C',
        13 => 'D',
        14 => 'E',
        15 => 'F',
        16 => 'G',
        17 => 'H',
        18 => 'I',
        19 => 'J',
        20 => 'K',
        21 => 'L',
        22 => 'M',
        23 => 'N',
        24 => 'O',
        25 => 'P',
        26 => 'Q',
        27 => 'R',
        28 => 'S',
        29 => 'T',
        30 => 'U',
        31 => 'V',
        32 => 'W',
        33 => 'X',
        34 => 'Y',
        35 => 'Z',
        _ => throw new ArgumentOutOfRangeException($"'{v}' is not a valid base32 value"),
    };
}
