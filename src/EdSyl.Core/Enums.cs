namespace EdSyl;

public static class Enums
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] T>(this T value)
        where T : struct, Enum
        => Enum<T>.GetDisplayName(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDisplayShortName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] T>(this T value)
        where T : struct, Enum
        => Enum<T>.GetDisplayShortName(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDisplayDescription<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] T>(this T value)
        where T : struct, Enum
        => Enum<T>.GetDisplayDescription(value);

    /// <summary> Check if the given type is enum or a nullable enum. </summary>
    /// <param name="value">Type to check.</param>
    public static bool IsEnum(this Type value)
    {
        return value.IsEnum || Nullable.GetUnderlyingType(value) is { IsEnum: true };
    }

    /// <summary> Check if the given type is enum or a nullable enum. </summary>
    /// <param name="value">Type to check.</param>
    /// <param name="type">Type itself or the underlying nullable type.</param>
    public static bool IsEnum(this Type value, out Type type)
    {
        type = value.IsEnum ? value : Nullable.GetUnderlyingType(value) ?? value;
        return type.IsEnum;
    }

    /// <summary> Check if the given type is enum or a nullable enum. </summary>
    /// <param name="value">Type to check.</param>
    /// <param name="type">Type itself or the underlying nullable type.</param>
    /// <param name="nullable">True when provided type is nullable; false otherwise.</param>
    public static bool IsEnum(this Type value, out Type type, out bool nullable)
    {
        if (!value.IsEnum && Nullable.GetUnderlyingType(value) is { } underlyingType)
        {
            nullable = true;
            type = underlyingType;
            return type.IsEnum;
        }

        type = value;
        nullable = false;
        return type.IsEnum;
    }
}
