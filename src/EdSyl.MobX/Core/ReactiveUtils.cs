using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace EdSyl.MobX;

public static class ReactiveUtils
{
    /// <summary> Report read access to a property with the given name. </summary>
    /// <param name="proxy">Reactive proxy.</param>
    /// <param name="value">Current value of the property.</param>
    /// <param name="name">Name of the property.</param>
    /// <typeparam name="T">Type of the property.</typeparam>
    public static T Read<T>(this Reactive proxy, T value, [CallerMemberName] string? name = default)
    {
        proxy.TrackRead(name!);
        return value;
    }

    /// <summary> Report modification if the value of the property with the given name has changed. </summary>
    /// <param name="proxy">Reactive proxy.</param>
    /// <param name="last">Last know property value before modification.</param>
    /// <param name="value">Current value of the property.</param>
    /// <param name="name">Name of the property.</param>
    /// <typeparam name="T">Type of the property.</typeparam>
    public static void Write<T>(this Reactive proxy, T last, T value, [CallerMemberName] string? name = default)
    {
        if (!Cache<T>.Comparer.Equals(last, value))
            proxy.TrackWrite(name!);
    }

    /// <summary> Get a reactive version of the given list. </summary>
    /// <param name="list">Original or reactive list.</param>
    /// <typeparam name="T">Type of the elements in a list.</typeparam>
    public static ListRx<T> AsReactive<T>(this IList<T> list)
        => list as ListRx<T> ?? new ListRx<T>(list);
}

file static class Cache<T>
{
    public static readonly EqualityComparer<T> Comparer = typeof(T).IsValueType
        ? EqualityComparer<T>.Default
        : new ReferenceEqualityComparer<T>();
}

file sealed class ReferenceEqualityComparer<T> : EqualityComparer<T>
{
    public override bool Equals(T? x, T? y) => ReferenceEquals(x, y);
    public override int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}
