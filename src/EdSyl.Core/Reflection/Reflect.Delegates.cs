namespace EdSyl.Reflection;

public static partial class Reflect
{
    /// <summary> Create an unbound delegate of this given type from the provided method. </summary>
    /// <param name="method">Method to create delegate for.</param>
    /// <typeparam name="T">Type of the delegate to create.</typeparam>
    public static T CreateDelegate<T>(this MethodInfo method) where T : Delegate
        => method.CreateDelegate<T>();

    /// <summary> Create an unbound delegate of this given type from the provided method. </summary>
    /// <param name="method">Method to create delegate for.</param>
    /// <param name="output">Field to store created delegate.</param>
    /// <typeparam name="T">Type of the delegate to create.</typeparam>
    public static void CreateDelegate<T>(this MethodInfo method, out T output) where T : Delegate
        => output = method.CreateDelegate<T>();

    /// <summary> Create a bound delegate of this given type from the provided method. </summary>
    /// <param name="method">Method to create delegate for.</param>
    /// <param name="instance">Instance the delegate should be bound with.</param>
    /// <typeparam name="T">Type of the delegate to create.</typeparam>
    public static T CreateDelegate<T>(this MethodInfo method, object instance) where T : Delegate
        => method.CreateDelegate<T>(instance);

    /// <summary> Create a bound delegate of this given type from the provided method. </summary>
    /// <param name="method">Method to create delegate for.</param>
    /// <param name="instance">Instance the delegate should be bound with.</param>
    /// <param name="output">Field to store created delegate.</param>
    /// <typeparam name="T">Type of the delegate to create.</typeparam>
    public static void CreateDelegate<T>(this MethodInfo method, object instance, out T output) where T : Delegate
        => output = method.CreateDelegate<T>(instance);
}
