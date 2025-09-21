using System.Runtime.CompilerServices;

namespace EdSyl.Razor;

public static class Styling
{
    /// <summary> Check if the component variant is active. </summary>
    /// <param name="current">Field storing the current variant value.</param>
    /// <param name="variant">Variant to look for.</param>
    public static bool Variance(string? current, [CallerMemberName] string? variant = null)
        => current == variant;

    /// <summary> Check if the component variant is active. </summary>
    /// <param name="current">Field storing the current variant value.</param>
    /// <param name="variant">Variant to look for.</param>
    public static bool Variance(ref string? current, [CallerMemberName] string? variant = null)
        => current == variant;

    /// <summary> Check if the component variant is active. </summary>
    /// <param name="current">Field storing the current variant value.</param>
    /// <param name="variant">Variant to look for.</param>
    public static bool VarianceDefault(ref string? current, [CallerMemberName] string? variant = null)
        => current == null || current == variant;

    /// <summary> Toggle the component variant. </summary>
    /// <param name="current">Field storing the current variant value.</param>
    /// <param name="enable">Whether to enable or disable the variant.</param>
    /// <param name="variant">Variant to toggle.</param>
    public static void Variance(ref string? current, bool enable, [CallerMemberName] string? variant = null)
    {
        if (enable) current = variant;
        else if (current == variant) current = default;
    }

    /// <summary> Toggle the component variant. </summary>
    /// <param name="current">Field storing the current variant value.</param>
    /// <param name="enable">Whether to enable or disable the variant.</param>
    /// <param name="variant">Variant to toggle.</param>
    public static string? Variance(string? current, bool enable, [CallerMemberName] string? variant = null)
    {
        if (enable) return variant;
        return current != variant ? current : default;
    }

    /// <summary> Toggle the class name at the given field. </summary>
    /// <param name="current">Field storing the current klass value.</param>
    /// <param name="enable">Whether to enable or disable the class.</param>
    /// <param name="klass">Class name to toggle.</param>
    public static void Variate(ref string? current, bool enable, [LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass)
    {
        if (enable) current = klass;
        else if (current == klass) current = default;
    }
}
