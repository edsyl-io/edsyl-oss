using static System.Runtime.CompilerServices.Unsafe;

namespace EdSyl;

public static class Flags
{
    /// <summary> Check whether the value has all bit flags set. </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="flags">Bitwise flags to check against.</param>
    /// <typeparam name="T">Type of enum flags.</typeparam>
    /// <returns>True if all flags set, false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When provided enum type is not supported.</exception>
    public static bool Has<T>(this T value, T flags) where T : unmanaged, Enum
    {
        var v = As<T, ulong>(ref value);
        var f = As<T, ulong>(ref flags);
        return (v & f) == f;
    }

    /// <summary> Toggle the value bits based on the bitwise flags. </summary>
    /// <param name="value">Reference to the enum value to modify.</param>
    /// <param name="flags">Bitwise flags to toggle.</param>
    /// <param name="on">Whether to toggle bits on or off.</param>
    /// <typeparam name="T">Type of enum flags.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">When provided enum type is not supported.</exception>
    public static void Toggle<T>(ref T value, T flags, bool on) where T : struct, Enum
    {
        var v = As<T, ulong>(ref value);
        var f = As<T, ulong>(ref flags);
        v = on ? v | f : v & ~f;
        value = As<ulong, T>(ref v);
    }

    /// <summary> Toggle the value bits based on the bitwise flags. </summary>
    /// <param name="value">Reference to the enum value to modify.</param>
    /// <param name="flags">Bitwise flags to toggle.</param>
    /// <param name="on">Whether to toggle bits on or off.</param>
    /// <typeparam name="T">Type of enum flags.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">When provided enum type is not supported.</exception>
    public static T Toggle<T>(this T value, T flags, bool on) where T : struct, Enum
    {
        var v = As<T, ulong>(ref value);
        var f = As<T, ulong>(ref flags);
        v = on ? v | f : v & ~f;
        return As<ulong, T>(ref v);
    }
}
