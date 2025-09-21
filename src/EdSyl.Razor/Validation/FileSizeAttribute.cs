using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace EdSyl.Razor.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FileSizeAttribute(long size) : ValidationAttribute
{
    private readonly ByteSize size = ByteSize.FromBytes(size);

    /// <summary> Limit of the file size in bytes. </summary>
    public long Size => size.Bits / 8;

    /// <inheritdoc />
    public override bool IsValid(object? value) => value switch
    {
        IBrowserFile file => file.Size <= size.Bytes,
        _ => true,
    };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
        => string.Format(CultureInfo.CurrentCulture, "File size exceeds the maximum limit of {0}", size);
}
