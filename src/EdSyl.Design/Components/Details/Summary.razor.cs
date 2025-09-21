namespace EdSyl.Design.Components;

[ClassName("summary")]
public class Summary : ComponentWithChildren
{
    private readonly object onClick;

    public Summary()
    {
        // use null receiver to avoid re-rendering
        onClick = (null as IHandleEvent).Callback(CreateClickHandler(this));
    }

    [Parameter]
    public RenderFragment? Marker { get; set; } = Templates<Marker>.Fragment;

    [CascadingParameter]
    public Details? Details { get; set; }

    /// <inheritdoc />
    protected override string Tag => Details is { Native: false } ? "div" : "summary";

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var tag = As ?? Tag;
        var native = tag == "summary";
        Attributes.Set("onclick", native ? null : onClick);

        builder.OpenElement(0, tag);
        builder.AddMultipleAttributes(1, Attributes);
        builder.AddContent(2, Marker);
        builder.AddContent(3, ChildContent);
        builder.AddElementReferenceCapture(4, Capture);
        builder.CloseElement();
    }

    private static Func<Task> CreateClickHandler(Summary self) => ()
        => self.Details is { NoToggle: false } details
            ? details.Toggle()
            : Task.CompletedTask;
}
