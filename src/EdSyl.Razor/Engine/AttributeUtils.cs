using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

public static class AttributeUtils
{
    private static readonly object True = true;
    private static readonly Dictionary<string, string> PreventDefaults = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> StopNavigations = new(StringComparer.OrdinalIgnoreCase);

    /// <summary> Toggles a boolean attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be toggled.</param>
    /// <param name="value">A boolean value determining attribute presence.</param>
    public static void Toggle(this IDictionary<string, object> attributes, string name, bool value)
    {
        if (value) attributes[name] = True;
        else attributes.Remove(name);
    }

    /// <summary> Add a boolean attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be toggled.</param>
    public static void Set(this IDictionary<string, object> attributes, string name)
    {
        attributes[name] = True;
    }

    /// <summary> Set the value of an attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to set; null to remove the attribute.</param>
    public static void Set(this IDictionary<string, object> attributes, string name, object? value)
    {
        if (value == null) attributes.Remove(name);
        else attributes[name] = value;
    }

    /// <summary> Set the value of an attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to set; null to remove the attribute.</param>
    public static void Set(this IDictionary<string, object> attributes, string name, bool? value)
    {
        if (value == null) attributes.Remove(name);
        else Toggle(attributes, name, value.Value);
    }

    /// <summary> Set the value of an attribute if not yet set. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to add.</param>
    public static bool TrySet(this IDictionary<string, object> attributes, string name, object? value)
        => value != null && attributes.TryAdd(name, value);

    /// <summary> Modify the value of an attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to set.</param>
    /// <returns>True when the attribute value has been changed; false if the attribute already had an equal value.</returns>
    public static bool Modify(this IDictionary<string, object> attributes, string name, bool value)
    {
        return value
            ? Modify(attributes, name, True)
            : attributes.Remove(name);
    }

    /// <summary> Modify the value of an attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to set; null to remove the attribute.</param>
    /// <returns>True when the attribute value has been changed; false if the attribute already had an equal value.</returns>
    public static bool Modify(this IDictionary<string, object> attributes, string name, object? value)
    {
        if (value == null) return attributes.Remove(name);
        attributes.TryGetValue(name, out var curr);
        attributes[name] = value;
        return !Equals(curr, value);
    }

    /// <summary> Modify the value of an attribute. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be modified.</param>
    /// <param name="value">Value of the attribute to set; null to remove the attribute.</param>
    /// <returns>True when the attribute value has been changed; false if the attribute already had an equal value.</returns>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static bool Modify(this Dictionary<string, object> attributes, string name, object? value)
    {
        if (value == null) return attributes.Remove(name);
        ref var current = ref GetValueRefOrAddDefault(attributes, name, out _);
        var differs = current != value;
        current = value;
        return differs;
    }

    /// <summary> Substitute an attribute by a given name with the provided replacement value. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the attribute to be substituted.</param>
    /// <param name="value">Field to hold the original attribute value.</param>
    /// <param name="replacement">Value to replace with.</param>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static void Substitute(this Dictionary<string, object> attributes, string name, ref object? value, object replacement)
    {
        ref var current = ref GetValueRefOrAddDefault(attributes, name, out var exists);
        if (exists && current != replacement) value = current;
        current = replacement;
    }

    /// <summary> Prevent the default action for a specified event. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the event to be affected.</param>
    /// <param name="value">True if the default action is to be prevented, otherwise false.</param>
    /// <remarks>https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Web/WebRenderTreeBuilderExtensions.cs</remarks>
    public static void PreventDefault(this IDictionary<string, object> attributes, string name, bool value = true)
    {
        ref var attribute = ref GetValueRefOrAddDefault(PreventDefaults, name, out _);
        attribute ??= "__internal_preventDefault_" + name;
        Toggle(attributes, attribute, value);
    }

    /// <summary> Stop the specified event from propagating beyond the current element. </summary>
    /// <param name="attributes">Attributes to modify.</param>
    /// <param name="name">The name of the event to be affected.</param>
    /// <param name="value">True if propagation should be stopped here, otherwise false.</param>
    /// <remarks>https://github.com/dotnet/aspnetcore/blob/main/src/Components/Web/src/Web/WebRenderTreeBuilderExtensions.cs</remarks>
    public static void StopPropagation(this IDictionary<string, object> attributes, string name, bool value = true)
    {
        ref var attribute = ref GetValueRefOrAddDefault(StopNavigations, name, out _);
        attribute ??= "__internal_stopPropagation_" + name;
        Toggle(attributes, attribute, value);
    }
}
