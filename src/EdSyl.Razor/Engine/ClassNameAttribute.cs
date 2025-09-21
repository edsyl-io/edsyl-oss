namespace EdSyl.Razor;

/// <summary> Declares the default class name to use for a component. </summary>
[BaseTypeRequired(typeof(IStyledComponent))]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ClassNameAttribute : Attribute
{
    [SuppressMessage("Style", "IDE0290", Justification = "Language Injection")]
    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor", Justification = "Language Injection")]
    public ClassNameAttribute([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass)
        => Klass = klass;

    /// <summary> Class name to use by default on a new component instance. </summary>
    public ClassName Klass { get; }
}
