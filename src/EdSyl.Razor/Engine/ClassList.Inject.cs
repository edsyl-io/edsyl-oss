using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

public partial class ClassList
{
    /// <summary>
    /// Replace a class instance on the following attributes by this instance.
    /// Pass existing class from attributes to <see cref="ToggleExternal" /> if any.
    /// </summary>
    /// <param name="attributes">Collection of component attributes.</param>
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public void InjectTo(Dictionary<string, object> attributes)
    {
        ref var klass = ref GetValueRefOrAddDefault(attributes, "class", out _);
        klass = klass != this ? ToggleExternal(klass?.ToString()) : this;
    }

    /// <inheritdoc cref="InjectTo(Dictionary{string, object}" />
    /// <returns> Collection of attributes with this instance injected as a class attribute. </returns>
    public IDictionary<string, object> InjectTo(IDictionary<string, object>? attributes)
    {
        if (attributes == null)
            return new Dictionary<string, object>(StringComparer) { { "class", this } };

        if (attributes.TryGetValue("class", out var klass) && klass != this)
            ToggleExternal(klass.ToString());

        attributes["class"] = this;
        return attributes;
    }

    /// <inheritdoc cref="InjectTo(IDictionary{string, object}" />
    public IReadOnlyDictionary<string, object> InjectTo(IReadOnlyDictionary<string, object>? attributes)
    {
        if (attributes == null)
            return new Dictionary<string, object>(StringComparer) { { "class", this } };

        if (attributes.TryGetValue("class", out var klass) && klass != this)
            ToggleExternal(klass.ToString());

        if (attributes is not IDictionary<string, object> dictionary)
            return new Dictionary<string, object>(attributes, StringComparer) { ["class"] = this };

        dictionary["class"] = this;
        return attributes;
    }

    /// <inheritdoc cref="InjectTo(IReadOnlyDictionary{string, object}" />
    /// <param name="dictionary">Mutable collection of attributes having this instance injected.</param>
    public IReadOnlyDictionary<string, object> InjectTo(IReadOnlyDictionary<string, object>? attributes, out IDictionary<string, object> dictionary)
    {
        attributes = InjectTo(attributes);
        dictionary = (IDictionary<string, object>)attributes;
        return attributes;
    }
}
