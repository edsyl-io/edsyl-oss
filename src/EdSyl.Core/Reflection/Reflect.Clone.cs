namespace EdSyl.Reflection;

public static partial class Reflect
{
    private static readonly Func<object, object> MemberwiseCloneDelegate = typeof(object)
        .RequireMethod(nameof(MemberwiseClone), BindingFlags.Instance | BindingFlags.NonPublic)
        .CreateDelegate<Func<object, object>>();

    /// <summary>
    /// Create a copy of the given object.
    /// Detects if the object implements <see cref="ICloneable" /> and uses it if so.
    /// Otherwise, uses <see cref="object.MemberwiseClone" /> method.
    /// </summary>
    /// <param name="source">Object to clone.</param>
    /// <typeparam name="T">Type of the cloned object.</typeparam>
    public static T Clone<T>(this T source) where T : class
    {
        return source is ICloneable cloneable
            ? (T)cloneable.Clone()
            : (T)MemberwiseCloneDelegate(source);
    }

    /// <inheritdoc cref="Clone{T}(T)" />
    /// <param name="source">Object to clone.</param>
    /// <param name="configure">Action to invoke on the cloned object.</param>
    public static T Clone<T>(this T source, Action<T> configure) where T : class
    {
        var clone = Clone(source);
        configure(clone);
        return clone;
    }
}
