namespace EdSyl.Encoding;

public static class Base52
{
    private const int Basis = 52;
    private const int UInt32Digits = 6;
    private const int UInt64Digits = 12;

    public static string Encode(int value)
    {
        return Encode((ulong)value, stackalloc char[UInt32Digits]).ToString();
    }

    public static string? Encode(int? value)
    {
        return value.HasValue
            ? Encode((ulong)value.Value, stackalloc char[UInt32Digits]).ToString()
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

    public static uint DecodeUInt32(ReadOnlySpan<char> chars)
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

    public static bool Equal(ulong? value, ReadOnlySpan<char> base62)
    {
        var chars = value != null
            ? Encode(value.Value, stackalloc char[UInt64Digits])
            : Span<char>.Empty;

        return chars.SequenceEqual(base62);
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
        'a' => 26,
        'b' => 27,
        'c' => 28,
        'd' => 29,
        'e' => 30,
        'f' => 31,
        'g' => 32,
        'h' => 33,
        'i' => 34,
        'j' => 35,
        'k' => 36,
        'l' => 37,
        'm' => 38,
        'n' => 39,
        'o' => 40,
        'p' => 41,
        'q' => 42,
        'r' => 43,
        's' => 44,
        't' => 45,
        'u' => 46,
        'v' => 47,
        'w' => 48,
        'x' => 49,
        'y' => 50,
        'z' => 51,
        _ => throw new ArgumentOutOfRangeException($"'{c}' is not a valid base52 character"),
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
        26 => 'a',
        27 => 'b',
        28 => 'c',
        29 => 'd',
        30 => 'e',
        31 => 'f',
        32 => 'g',
        33 => 'h',
        34 => 'i',
        35 => 'j',
        36 => 'k',
        37 => 'l',
        38 => 'm',
        39 => 'n',
        40 => 'o',
        41 => 'p',
        42 => 'q',
        43 => 'r',
        44 => 's',
        45 => 't',
        46 => 'u',
        47 => 'v',
        48 => 'w',
        49 => 'x',
        50 => 'y',
        51 => 'z',
        _ => throw new ArgumentOutOfRangeException($"'{v}' is not a valid base52 value"),
    };
}
