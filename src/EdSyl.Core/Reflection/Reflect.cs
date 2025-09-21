using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.Reflection;

public static partial class Reflect
{
    /// <summary> Default binding flags. </summary>
    private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary> Get a field with the given name. </summary>
    /// <param name="reflect">Reflections container.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="flags">Binding attributes used to control the search.</param>
    /// <returns>Property reflection.</returns>
    /// <exception cref="MissingMemberException">When no property found by the given name.</exception>
    public static FieldInfo RequireField([DynamicallyAccessedMembers(PublicFields | NonPublicFields)] this IReflect reflect, string name, BindingFlags flags = Flags)
    {
        return reflect.GetField(name, flags)
            ?? throw new MissingMemberException(reflect.UnderlyingSystemType.FullName, name);
    }

    /// <summary> Get property with the given name. </summary>
    /// <param name="reflect">Reflections container.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="flags">Binding attributes used to control the search.</param>
    /// <returns>Property reflection.</returns>
    /// <exception cref="MissingMemberException">When no property found by the given name.</exception>
    public static PropertyInfo RequireProperty([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties)] this IReflect reflect, string name, BindingFlags flags = Flags)
    {
        return reflect.GetProperty(name, flags)
            ?? throw new MissingMemberException(reflect.UnderlyingSystemType.FullName, name);
    }

    /// <summary> Get a method with the given name. </summary>
    /// <param name="reflect">Reflections container.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="flags">Binding attributes used to control the search.</param>
    /// <returns>Method reflection.</returns>
    /// <exception cref="MissingMemberException">When no method found by the given name.</exception>
    public static MethodInfo RequireMethod([DynamicallyAccessedMembers(PublicMethods | NonPublicMethods)] this IReflect reflect, string name, BindingFlags flags = Flags)
    {
        return reflect.GetMethod(name, flags)
            ?? throw new MissingMemberException(reflect.UnderlyingSystemType.FullName, name);
    }

    /// <summary> Get method with the given name. </summary>
    /// <param name="reflect">Reflections container.</param>
    /// <param name="name">Name of the property.</param>
    /// <param name="types">Types of the method parameters to search for.</param>
    /// <param name="flags">Binding attributes used to control the search.</param>
    /// <returns>Method reflection.</returns>
    /// <exception cref="MissingMemberException">When no method found by the given name.</exception>
    public static MethodInfo RequireMethod([DynamicallyAccessedMembers(PublicMethods | NonPublicMethods)] this IReflect reflect, string name, Type[] types, BindingFlags flags = Flags)
    {
        return reflect.GetMethod(name, flags, binder: null, types, modifiers: null)
            ?? throw new MissingMemberException(reflect.UnderlyingSystemType.FullName, name);
    }

    /// <summary> Get a member representing property or field accessor. </summary>
    /// <param name="reflect">Reflections container.</param>
    /// <param name="name">Name of the field or property.</param>
    /// <param name="flags">Binding attributes used to control the search.</param>
    /// <returns>Property or field accessor.</returns>
    public static MemberInfo? GetAccessor([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicFields | NonPublicFields)] this IReflect reflect, string name, BindingFlags flags = Flags)
    {
        return (MemberInfo?)reflect.GetProperty(name, flags)
            ?? reflect.GetField(name, flags);
    }

    /// <inheritdoc cref="GetAccessor(IReflect, string, BindingFlags)" />
    /// <exception cref="MissingMemberException">When no field or property found by the given name.</exception>
    public static MemberInfo RequireAccessor([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicFields | NonPublicFields)] this IReflect reflect, string name, BindingFlags flags = Flags)
    {
        return (MemberInfo?)reflect.GetProperty(name, flags)
            ?? reflect.GetField(name, flags)
            ?? throw new MissingMemberException(reflect.UnderlyingSystemType.FullName, name);
    }
}
