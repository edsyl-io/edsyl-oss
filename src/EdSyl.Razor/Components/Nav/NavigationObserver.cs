using Microsoft.AspNetCore.Components.Routing;

namespace EdSyl.Razor.Components;

public sealed class NavigationObserver : ComponentBase, IDisposable
{
    [Parameter]
    [EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Inject]
    public required NavigationManager Navigation { get; set; }

    private readonly EventHandler<LocationChangedEventArgs> locationChangeHandler;

    public NavigationObserver()
        => locationChangeHandler = (_, _) => StateHasChanged();

    /// <inheritdoc />
    public void Dispose()
        => Navigation.LocationChanged -= locationChangeHandler;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => ChildContent(builder);

    /// <inheritdoc />
    protected override void OnInitialized()
        => Navigation.LocationChanged += locationChangeHandler;
}
