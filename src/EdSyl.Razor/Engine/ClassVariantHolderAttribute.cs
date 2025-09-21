namespace EdSyl.Razor;

/// <summary>
/// Represents a member holding the value of the component variant to apply.
/// Must be a <see cref="string" /> property or field.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ClassVariantHolderAttribute : Attribute
{
    /// <summary> Default member to use when the holder value is null. </summary>
    public string? Default { get; set; }
}
