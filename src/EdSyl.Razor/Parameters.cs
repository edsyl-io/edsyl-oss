using System.Reflection;
using System.Runtime.CompilerServices;
using EdSyl.Reflection;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

public static class Parameters
{
    private static readonly Dictionary<Type, ParameterInfo[]> Cache = [];

    /// <summary> Get all values of all properties marked with <see cref="ParameterAttribute" />. </summary>
    /// <param name="component">Component to extract parameters from.</param>
    /// <returns>A map of parameter names and values.</returns>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static Dictionary<string, object>? GetParameters(this IComponent? component)
    {
        return component != null
            ? Evaluate(component, Of(component.GetType()))
            : default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ParameterInfo[] Of(Type type)
    {
#pragma warning disable IL2067
        return GetValueRefOrAddDefault(Cache, type, out _) ??= Parse(type).ToArray();
#pragma warning restore IL2067
    }

    private static Dictionary<string, object> Evaluate(IComponent component, ParameterInfo[] parameters)
    {
        var values = new Dictionary<string, object>(parameters.Length);
        foreach (var parameter in parameters)
            values.Add(parameter.Name, parameter.Getter(component));

        return values;
    }

    private static IEnumerable<ParameterInfo> Parse([DynamicallyAccessedMembers(PublicProperties)] Type type)
    {
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            if (property.GetCustomAttribute<ParameterAttribute>() is { CaptureUnmatchedValues: false })
                yield return new(property);
    }

    private readonly struct ParameterInfo(PropertyInfo property)
    {
        /// <summary> Name of the parameter. </summary>
        public readonly string Name = property.Name;

        /// <summary> Function to evaluate the parameter value from the component. </summary>
        public readonly Func<object, object> Getter = property.Getter<object, object>();
    }
}
