using System.ComponentModel.DataAnnotations;

namespace EdSyl.Razor.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DenyNullAttribute() : ValidationAttribute(static () => "The {0} field is required.")
{
    /// <inheritdoc />
    public override bool IsValid(object? value)
        => value is not null;
}
