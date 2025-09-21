using Cortex.Net.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace EdSyl.MobX.Razor;

/// <summary>
/// Renders the given render function and automatically re-renders it
/// once one of the observables used in the render function changes.
/// </summary>
/// <remarks>https://mobx.js.org/api.html#observer</remarks>
public sealed class Observer : ComponentBase, IDisposable
{
    [Parameter]
    [EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    private readonly ObserverObject observer;

    public Observer()
        => observer = MobX.Observer(nameof(Observer), Render, StateHasChanged);

    /// <inheritdoc />
    public void Dispose()
        => observer.Dispose();

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => observer.BuildRenderTree(builder);

    private void Render(RenderTreeBuilder builder)
        => ChildContent(builder);
}
