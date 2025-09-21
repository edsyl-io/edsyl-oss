using System.Globalization;
using System.Text;

namespace EdSyl;

public static class Strings
{
    /// <summary> Returns null if the provided string is empty. </summary>
    /// <param name="input">String to check.</param>
    public static string? NullIfEmpty(this string? input)
    {
        return string.IsNullOrEmpty(input) ? null : input;
    }

    /// <summary> Returns null if the provided string is null or blank. </summary>
    /// <param name="input">String to check.</param>
    public static string? NullIfBlank(this string? input)
    {
        return string.IsNullOrWhiteSpace(input) ? null : input;
    }

    /// <summary> Convert the input string to a kebab-case, having all letters lowercase separated by dashes. </summary>
    /// <param name="value">String to convert.</param>
    [SuppressMessage("Globalization", "CA1308", Justification = "kebab-case always lowercase")]
    public static string ToKebabCase(this string value)
    {
        return string.Join('-', value.ToLower(CultureInfo.InvariantCulture).Split(' '));
    }

    /// <summary> Get a string representation of the corresponding objects in the specified format. </summary>
    /// <param name="format">A parsed composite format string.</param>
    /// <param name="a">The first object to format.</param>
    /// <typeparam name="T1">Type of the first argument to format.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1>(this CompositeFormat format, T1 a)
        => string.Format(null, format, a);

    /// <summary> Get a string representation of the corresponding objects in the specified format. </summary>
    /// <param name="format">A parsed composite format string.</param>
    /// <param name="a">The first object to format.</param>
    /// <param name="b">The second object to format.</param>
    /// <typeparam name="T1">Type of the first argument to format.</typeparam>
    /// <typeparam name="T2">Type of the second argument to format.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2>(this CompositeFormat format, T1 a, T2 b)
        => string.Format(null, format, a, b);

    /// <summary> Check if the given string contains any printable characters. </summary>
    /// <param name="input">String for printing.</param>
    public static bool IsPrintable(this string? input)
    {
        return !string.IsNullOrWhiteSpace(input);
    }

    /// <summary> Get printable string by removing all leading and trailing white-space character. </summary>
    /// <param name="input">String for printing.</param>
    public static string? Print(this string? input)
    {
        return input?.Trim() is { Length: > 0 } value ? value : null;
    }

    /// <summary> Splits a string into substrings at specified index. </summary>
    /// <param name="input">String to split.</param>
    /// <param name="index">Zero-based index at which to split the string.</param>
    public static (string, string) Split(this string input, int index)
    {
        return (input[..index], input[index..]);
    }

    /// <summary> Get printable string by removing all leading and trailing white-space character. </summary>
    /// <param name="input">String for printing.</param>
    /// <param name="fallback">Fallback string to return when input doesn't have printable characters.</param>
    public static string Print(this string? input, string fallback)
    {
        return input?.Trim() is { Length: > 0 } value ? value : fallback;
    }

    /// <summary> Check a specified string occurs within this string, using the specified comparison rules. </summary>
    /// <param name="input">String to search.</param>
    /// <param name="value">String to locate within <paramref name="input" />.</param>
    /// <param name="options">String comparison options.</param>
    public static bool Match(this string? input, string value, CompareOptions options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols)
    {
        return input?.Length > 0 && CultureInfo.InvariantCulture.CompareInfo.IndexOf(input, value, options) >= 0;
    }

    /// <summary>Splits a string into substrings based on a specified delimiting character and options.</summary>
    /// <param name="input">String to enumerate.</param>
    /// <param name="separator">A character that delimits the substrings in this string.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim substrings and include empty substrings.</param>
    public static StringSplitEnumerator EnumerateSplits(this string? input, char separator, StringSplitOptions options = default)
        => new(input, separator, options);

    /// <summary> Searches a sorted array for a value. </summary>
    /// <param name="array">Sorted array to search.</param>
    /// <param name="value">Value to search for.</param>
    public static int BinarySearch(this string[] array, ReadOnlySpan<char> value)
    {
        var lo = 0;
        var hi = array.Length - 1;
        while (lo <= hi)
        {
            var i = lo + ((hi - lo) >> 1);
            var c = array[i].AsSpan().SequenceCompareTo(value);
            if (c < 0) lo = i + 1;
            else if (c > 0) hi = i - 1;
            else return i;
        }

        return ~lo;
    }

    /// <summary> Searches a sorted array for a value. </summary>
    /// <param name="array">Sorted array to search.</param>
    /// <param name="value">Value to search for.</param>
    /// <param name="index">Zero-based index of the element if found; negative value otherwise.</param>
    /// <returns>True if the element found; false otherwise.</returns>
    public static bool BinarySearch(this string[] array, ReadOnlySpan<char> value, out int index)
    {
        var lo = 0;
        var hi = array.Length - 1;
        while (lo <= hi)
        {
            index = lo + ((hi - lo) >> 1);
            var c = array[index].AsSpan().SequenceCompareTo(value);
            if (c < 0) lo = index + 1;
            else if (c > 0) hi = index - 1;
            else return true;
        }

        index = ~lo;
        return false;
    }
}
