namespace EdSyl.Reflection;

public static partial class Reflect
{
    /// <summary>Get unbound getter and setter the provided member. </summary>
    /// <param name="member">Member to access.</param>
    /// <param name="getter">Variable to hold the member getter.</param>
    /// <param name="setter">Variable to hold the member setter.</param>
    /// <typeparam name="T">Type of the object instance declaring a member.</typeparam>
    /// <typeparam name="TValue">Type of the member value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Accessors<T, TValue>(this MemberInfo member, out Func<T, TValue> getter, out Action<T, TValue> setter)
    {
        getter = Getter<T, TValue>(member);
        setter = Setter<T, TValue>(member);
    }

    /// <summary> Get unbound delegate allowing getting value of the provided member. </summary>
    /// <param name="member">Member to access.</param>
    /// <param name="getter">Variable to hold member getter.</param>
    /// <typeparam name="T">Type of the object instance declaring a member.</typeparam>
    /// <typeparam name="TResult">Type of the value returned by the field.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Getter<T, TResult>(this MemberInfo member, out Func<T, TResult> getter)
    {
        getter = Getter<T, TResult>(member);
    }

    /// <summary> Get unbound delegate allowing getting value of the provided member. </summary>
    /// <param name="member">Member to access.</param>
    /// <typeparam name="T">Type of the object instance declaring a member.</typeparam>
    /// <typeparam name="TResult">Type of the value returned by the member.</typeparam>
    /// <returns>Member accessor.</returns>
    /// <exception cref="ArgumentException">When <paramref name="member" /> is not a property or field.</exception>
    public static Func<T, TResult> Getter<T, TResult>(this MemberInfo member)
    {
        var instance = Expression.Parameter(typeof(T));
        var access = MakeMemberAccessTypeAs(instance, member, typeof(T));
        var convert = Expression.Convert(access, typeof(TResult));
        return Expression.Lambda<Func<T, TResult>>(convert, instance).Compile();
    }

    /// <summary> Get unbound delegate allowing setting value of the provided member. </summary>
    /// <param name="member">Member to access.</param>
    /// <param name="setter">Variable to hold field setter.</param>
    /// <typeparam name="T">Type of the object instance declaring a member.</typeparam>
    /// <typeparam name="TValue">Type of the setter input value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Setter<T, TValue>(this MemberInfo member, out Action<T, TValue> setter)
    {
        setter = Setter<T, TValue>(member);
    }

    /// <summary> Get unbound delegate allowing setting of the provided member. </summary>
    /// <param name="member">Member to access.</param>
    /// <typeparam name="T">Type of the object instance declaring a member.</typeparam>
    /// <typeparam name="TValue">Type of the setter input value.</typeparam>
    /// <returns>Member accessor.</returns>
    /// <exception cref="ArgumentException">When <paramref name="member" /> is not a property or field.</exception>
    public static Action<T, TValue> Setter<T, TValue>(this MemberInfo member)
    {
        var instance = Expression.Parameter(typeof(T));
        var value = Expression.Parameter(typeof(TValue));
        var access = MakeMemberAccessTypeAs(instance, member, typeof(T));
        var assign = Expression.Assign(access, value);
        return Expression.Lambda<Action<T, TValue>>(assign, instance, value).Compile();
    }

    private static MemberExpression MakeMemberAccessTypeAs(Expression instance, MemberInfo member, Type type)
    {
        var declaringType = member.DeclaringType!;
        if (!declaringType.IsAssignableFrom(type))
            instance = Expression.TypeAs(instance, declaringType);

        return Expression.MakeMemberAccess(instance, member);
    }
}
