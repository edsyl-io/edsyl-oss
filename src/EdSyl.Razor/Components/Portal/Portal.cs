namespace EdSyl.Razor.Components;

/// <summary>
/// Declares a content to render into <see cref="Slot" />.
/// </summary>
public sealed partial class Portal : IComponent, IDisposable
{
    private string? name;

    /// <summary> Globally unique name of the content. </summary>
    [Parameter]
    [EditorRequired]
    public string Name
    {
        get => name ?? string.Empty;
        [MemberNotNull(nameof(name))]
        set
        {
            if (name == value) return;
            Unregister();
            name = value;
            Register();
        }
    }

    /// <summary> Content to render into <see cref="Slot" />. </summary>
    [Parameter]
    [EditorRequired]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    void IComponent.Attach(RenderHandle renderHandle) { }

    /// <inheritdoc />
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        Slot.Render(name, ChildContent ?? Templates.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Unregister();
    }

    private void Register()
    {
        if (name != null)
            Portals[name] = this;
    }

    private void Unregister()
    {
        if (name != null && Portals.TryGetValue(name, out var portal) && this == portal)
        {
            Slot.Render(name, Templates.Empty);
            Portals.Remove(name);
        }
    }
}

public sealed partial class Portal
{
    private static readonly Dictionary<string, Portal> Portals = [];

    internal static bool TryGetFragment(string? name, [MaybeNullWhen(false)] out RenderFragment fragment)
    {
        if (name != null && Portals.TryGetValue(name, out var portal))
        {
            fragment = portal.ChildContent ?? Templates.Empty;
            return true;
        }

        fragment = default;
        return false;
    }
}
