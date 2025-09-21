using System.ComponentModel.DataAnnotations;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl;

[SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Caching per generic type")]
[SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Caching per generic type")]
[SuppressMessage("Design", "MA0018:Do not declare static members on generic types", Justification = "Caching per generic type")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Enum<T> is not a suffix")]
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Enum<T> is not a keyword")]
[SuppressMessage("Naming", "CS8714:Nullability of type argument doesn't match 'notnull' constraint.", Justification = "Weak typing support")]
public static class Enum<T>
{
    /// <summary> Type holding enum members. </summary>
    [DynamicallyAccessedMembers(PublicFields | NonPublicFields)]
    public static readonly Type Type = default!;

    /// <summary> Whether type represent a <see cref="Enum" />. </summary>
    public static readonly bool IsEnum = typeof(T).IsEnum(out Type);

    /// <summary> Array of all enum values. </summary>
    public static readonly T[] Values = IsEnum
        ? Enum.GetValues(Type).Cast<T>().ToArray()
        : Array.Empty<T>();

    /// <summary> Array of all enum names. </summary>
    public static readonly string[] Names = IsEnum
        ? Enum.GetNames(Type)
        : Array.Empty<string>();

    private static readonly Dictionary<T, int> Orders = new();
    private static readonly Dictionary<T, string> NameCache = new();
    private static readonly Dictionary<T, DisplayAttribute> DisplayAttributes = new();

    static Enum()
    {
        if (IsEnum)
        {
            foreach (var field in Type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var value = (T)field.GetValue(null)!;
                if (field.GetCustomAttribute<DisplayAttribute>() is { } display)
                {
                    DisplayAttributes[value] = display;
                    if (display.GetOrder() is { } order)
                        Orders[value] = order;
                }
            }

            if (Orders.Count > 1)
                Array.Sort(Values, OrderComparer.Default);

            for (var i = 0; i < Values.Length; i++)
                NameCache[Values[i]] = Names[i];
        }

        Orders.TrimExcess();
        NameCache.TrimExcess();
        DisplayAttributes.TrimExcess();
    }

    public static string GetDisplayDescription(T value)
    {
        if (!IsEnum)
            return value?.ToString() ?? string.Empty;

        if (DisplayAttributes.TryGetValue(value, out var attribute))
        {
            if (attribute.GetDescription() is { Length: > 0 } description)
                return description;

            if (attribute.GetName() is { Length: > 0 } name)
                return name;
        }

        return GetName(value);
    }

    public static string GetDisplayName(T value)
    {
        if (!IsEnum)
            return value?.ToString() ?? string.Empty;

        if (DisplayAttributes.TryGetValue(value, out var attribute))
            if (attribute.GetName() is { Length: > 0 } name)
                return name;

        return GetName(value);
    }

    public static string GetDisplayShortName(T value)
    {
        if (!IsEnum)
            return value?.ToString() ?? string.Empty;

        if (DisplayAttributes.TryGetValue(value, out var attribute))
            if (attribute.GetName() is { Length: > 0 } shortName)
                return shortName;

        return GetName(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetName(T value)
    {
        return NameCache.TryGetValue(value, out var name) ? name : string.Empty;
    }

    private class OrderComparer : IComparer<T>
    {
        public static readonly OrderComparer Default = new();

        public int Compare(T? x, T? y)
        {
            return Orders.GetValueOrDefault(x!)
                 - Orders.GetValueOrDefault(y!);
        }
    }
}
