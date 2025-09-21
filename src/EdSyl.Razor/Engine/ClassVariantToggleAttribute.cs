namespace EdSyl.Razor;

/// <summary>
/// Represents a toggle to activate the component variant.
/// Must be a computed property that writes it's name to the <see cref="ClassVariantHolderAttribute" /> field.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ClassVariantToggleAttribute : Attribute
{
    [SuppressMessage("Style", "IDE0290", Justification = "Language Injection")]
    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor", Justification = "Language Injection")]
    public ClassVariantToggleAttribute([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass)
        => Klass = klass;

    /// <summary> Class name to use when the property is on. </summary>
    public ClassName Klass { get; }
}
