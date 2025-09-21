using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace EdSyl.Razor.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FileContentTypeAttribute(params string[] types) : ValidationAttribute
{
    private readonly HashSet<string> types = new(types);

    /// <summary> List of supported content types. </summary>
    public ICollection<string> Types => types;

    /// <inheritdoc />
    public override bool IsValid(object? value) => value switch
    {
        IBrowserFile file => types.Contains(file.ContentType),
        _ => true,
    };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => "File type is not supported";
}
