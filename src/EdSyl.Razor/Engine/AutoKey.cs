using System.Runtime.CompilerServices;
using EdSyl.Encoding;

namespace EdSyl.Razor;

/// <summary> Automatically generate identifier to be used as an HTML attribute. </summary>
public sealed class AutoKey
{
    private readonly string id;

    /// <summary> Convert to string identifier. </summary>
    /// <param name="key">Key instance to convert.</param>
    /// <returns>Identifier to be used as an HTML attribute.</returns>
    public static implicit operator string(AutoKey key) => key.id;

    /// <summary> CSS selector for this element. </summary>
    [field: MaybeNull]
    public string Ref => field ??= "#" + id;

    /// <summary>Initializes a new instance of the <see cref="AutoKey" /> class. </summary>
    public AutoKey() => id = Compute(this);

    /// <summary>Initializes a new instance of the <see cref="AutoKey" /> class. </summary>
    /// <param name="any">Any reference type object.</param>
    public AutoKey(object any) => id = Compute(any);

    /// <inheritdoc />
    public override string ToString() => id;

    /// <summary> Compute an HTML attribute identifier for a given component.  </summary>
    /// <param name="any">Any reference type object.</param>
    public static string Compute(object any)
        => Base52.Encode(RuntimeHelpers.GetHashCode(any));
}
