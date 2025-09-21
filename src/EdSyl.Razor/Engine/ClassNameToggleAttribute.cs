namespace EdSyl.Razor;

/// <summary> Decorator for boolean properties to automatically render CSS class based its current state. </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ClassNameToggleAttribute : Attribute
{
    [SuppressMessage("Style", "IDE0290", Justification = "Language Injection")]
    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor", Justification = "Language Injection")]
    public ClassNameToggleAttribute([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? on)
        => On = on;

    [SuppressMessage("Style", "IDE0290", Justification = "Language Injection")]
    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor", Justification = "Language Injection")]
    public ClassNameToggleAttribute(
        [LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? on = null,
        [LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? off = null)
    {
        On = on;
        Off = off;
    }

    /// <summary> Class to use when the property is on. </summary>
    public ClassName On { get; }

    /// <summary> Class to use when the property is off. </summary>
    public ClassName Off { get; }
}
