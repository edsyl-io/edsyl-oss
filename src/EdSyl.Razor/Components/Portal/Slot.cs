namespace EdSyl.Razor.Components;

/// <summary>
/// Renders content of the <see cref="Portal" />.
/// </summary>
public sealed partial class Slot : IComponent, IDisposable
{
    private string? name;
    private RenderHandle handle;

    /// <summary>
    /// Name of the <see cref="Portal" /> to render.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string Name
    {
        get => name!;
        [MemberNotNull(nameof(name))]
        set
        {
            if (name == value) return;
            Unregister();
            name = value;
            Register();
        }
    }

    /// <inheritdoc />
    void IComponent.Attach(RenderHandle renderHandle)
    {
        handle = handle.IsInitialized
            ? throw new InvalidOperationException("The render handle is already set!")
            : renderHandle;
    }

    /// <inheritdoc />
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        if (Portal.TryGetFragment(name, out var fragment))
            handle.Render(fragment);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Unregister();
    }

    private void Register()
    {
        if (name == null) return;
        if (Registry.TryGetValue(name, out var slots)) slots.Add(this);
        else Registry[name] = [this];
    }

    private void Unregister()
    {
        if (name == null || !Registry.TryGetValue(name, out var slots))
            return;

        if (slots.Remove(this) && slots.Count < 1)
            Registry.Remove(name);
    }
}

public sealed partial class Slot
{
    private static readonly Dictionary<string, List<Slot>> Registry = [];

    public static void Render(string? name, RenderFragment fragment)
    {
        if (name != null && Registry.TryGetValue(name, out var slots))
            foreach (var slot in slots)
                slot.handle.Render(fragment);
    }
}
