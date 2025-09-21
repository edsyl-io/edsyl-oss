using static EdSyl.Razor.Interop.Js;

namespace EdSyl.Razor.Components;

[SuppressMessage("Design", "CA1063", Justification = "Dispose(true) is not necessary for Blazor components")]
public class Component : ComponentBase, IStyledComponent, IDisposable
{
    private string? id;
    private ILifecycle? lifecycle;
    private RenderHierarchy? hierarchy;

    /// <summary> Name of the HTML tag to use. </summary>
    [Parameter]
    public string? As { get; set; }

    /// <inheritdoc />
    public ClassList Class { get; }

    /// <summary> HTML element reference. </summary>
    public ElementReference Element
    {
        get;
        protected set
        {
            if (field.EqualTo(value)) field = value;
            else OnElementReferenceChange(field = value);
        }
    }

    /// <summary> Lifecycle of the current component. </summary>
    public ILifecycle Lifecycle => lifecycle ??= ILifecycle.New();

    /// <summary> Unmatched attributes for the root element. </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public Dictionary<string, object> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary> An action to be invoked whenever the element reference value changes. </summary>
    [field: MaybeNull]
    protected Action<ElementReference> Capture => field ??= value => Element = value;

    /// <summary> Default tag to use if <see cref="As" /> is empty. </summary>
    protected virtual string Tag => "div";

    /// <inheritdoc />
    public Component() => Class = ClassList.For(this);

    /// <summary> Whether to render a component or not. </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary> Children content to include in the element if applicable. </summary>
    [SuppressMessage("Roslynator", "RCS1140", Justification = "internal")]
    public virtual RenderFragment? ChildContent
    {
        get => hierarchy?.ChildContent;
        set => throw new InvalidOperationException("Unable to set ChildContent of Component.");
    }

    /// <summary> Hide by setting <see cref="Visible" /> to false. </summary>
    public void Hide() => ToggleVisible(false);

    /// <summary> Show by setting <see cref="Visible" /> to false. </summary>
    public void Show() => ToggleVisible(true);

    /// <summary> Toggle <see cref="Visible" />. </summary>
    public void ToggleVisible() => ToggleVisible(!Visible);

    /// <summary> Toggle <see cref="Visible" />. </summary>
    /// <param name="visible">Value to set for the <see cref="Visible" />.</param>
    public virtual void ToggleVisible(bool visible)
    {
        if (Visible == visible) return;
        Visible = visible;
        StateHasChanged();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        lifecycle?.Dispose();
        lifecycle = ILifecycle.Dead;
        OnDestroy();
    }

    /// <inheritdoc />
    public override string ToString() => id ??= AutoKey.Compute(this);

    /// <summary> Render an instance of <see cref="Component" />. </summary>
    /// <param name="self">Component to render.</param>
    /// <param name="builder">Renderer to receive the content.</param>
    public static void Render(Component self, RenderTreeBuilder builder)
    {
        if (!self.Visible) return;
        builder.OpenElement(0, self.As ?? self.Tag);
        builder.AddMultipleAttributes(1, self.Attributes);
        builder.AddContent(2, self.ChildContent);
        builder.AddElementReferenceCapture(3, self.Capture);
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Class.Render(this);
        Class.InjectTo(Attributes);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => Render(this, builder);

    /// <summary> Occurs when the rendered element reference value changes. </summary>
    /// <param name="el">Element reference.</param>
    protected virtual void OnElementReferenceChange(ElementReference el) { }

    /// <summary> Occurs when the component is getting destroyed. </summary>
    protected virtual void OnDestroy() { }

    /// <summary> Configure this component to render itself as a child content of the provided fragment. </summary>
    /// <param name="fragment">Fragment to render before self content.</param>
    /// <example>
    /// <c>WrapBy(base.BuildRenderTree)</c>
    /// </example>
    protected RenderHierarchy WrapBy(RenderFragment fragment)
    {
        // TODO: configure via render method attribute
        hierarchy ??= new(this);
        return hierarchy.WrapBy(fragment);
    }

    /// <summary> Configure this component to render itself as a child content of the provided fragment. </summary>
    /// <param name="fragment">Fragment to render this component.</param>
    protected RenderHierarchy WrapBy(RenderFragment<RenderFragment?> fragment)
    {
        hierarchy ??= new(this);
        return hierarchy.WrapBy(fragment);
    }

    /// <summary> Configure this component to render itself as a child content of the provided component. </summary>
    /// <param name="component">Type of the component to use as a parent.</param>
    protected RenderHierarchy WrapBy([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component)
    {
        // TODO: configure via class attribute
        hierarchy ??= new(this);
        return hierarchy.WrapBy(component);
    }

    /// <summary> Configure this component to use itself as a cascading parameter. </summary>
    /// <typeparam name="T">Type of the cascading value.</typeparam>
    /// <param name="value">Value to be provided for children.</param>
    protected RenderHierarchy WrapByCascade<T>(T value)
    {
        // TODO: configure via class attribute
        hierarchy ??= new(this);
        return hierarchy.WrapByCascade(value);
    }

    /// <summary> Configure this component to use itself as a cascading parameter. </summary>
    protected RenderHierarchy WrapByCascade()
    {
        // TODO: configure via class attribute
        hierarchy ??= new(this);
        return hierarchy.WrapByCascade();
    }
}

public class ComponentWithChildren : Component
{
    private RenderFragment? childContent;

    /// <summary> Text to render when no <see cref="ChildContent" /> provided. </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary> Children content to include in the element if applicable. </summary>
    [Parameter]
    public override RenderFragment? ChildContent
    {
        get => base.ChildContent ?? childContent;
        set => childContent = value;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        childContent ??= builder =>
        {
            if (!string.IsNullOrWhiteSpace(Text))
                builder.AddContent(0, Text);
        };
    }
}
