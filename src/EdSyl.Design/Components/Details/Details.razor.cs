namespace EdSyl.Design.Components;

[ClassName("details")]
public class Details : ComponentWithChildren
{
    private readonly object onToggle;

    /// <inheritdoc />
    public Details()
    {
        // use null receiver to avoid re-rendering
        onToggle = (null as IHandleEvent).Callback(CreateToggleHandler(this));
    }

    /// <summary> Set to reveal disclose content. </summary>
    /// <remarks> https://developer.mozilla.org/en-US/docs/Web/HTML/Element/details#open </remarks>
    [Parameter]
    public bool Open { get; set; }

    /// <summary> Occurs when <see cref="Open" /> has been changed. </summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary> Content to disclose when details are open. </summary>
    [Parameter]
    public RenderFragment? DiscloseContent { get; set; }

    /// <summary> Set to prevent toggling without disabling the element. </summary>
    [Parameter]
    public bool NoToggle { get; set; }

    /// <summary> Whether displayed as a native HTML Details element. </summary>
    public bool Native => (As ?? Tag) == "details";

    /// <inheritdoc />
    protected override string Tag => DiscloseContent != null ? "div" : "details";

    /// <summary> Toggle <see cref="Open" /> state. </summary>
    public Task Toggle() => SetOpen(!Open);

    /// <summary> Toggle <see cref="Open" /> state. </summary>
    /// <param name="open">Value to set.</param>
    public Task Toggle(bool open) => Open != open ? SetOpen(open) : Task.CompletedTask;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var tag = As ?? Tag;
        var native = tag == "details";

        Attributes.Toggle("open", Open);
        Attributes.Set("onDetailsToggle", native ? onToggle : default);
        Attributes.Set("onclick", NoToggle ? "return false" : default);

        builder.OpenElement(0, tag);
        builder.AddMultipleAttributes(1, Attributes);
        builder.AddElementReferenceCapture(2, Capture);

        builder.Cascade(3, this, ChildContent);
        if (Open) builder.AddContent(10, DiscloseContent);
        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && Open)
            SyncOpen();
    }

    private Task SetOpen(bool open)
    {
        Open = open;
        SyncOpen();
        StateHasChanged();
        return OpenChanged.InvokeAsync(Open);
    }

    private void SyncOpen()
    {
        // FIXME: for some reason, Blazor doesn't supply the "open" attribute after re-rendering
        if (Native) Element.Attr("open", Open);
    }

    private static Func<ToggleEventArgs, Task> CreateToggleHandler(Details self) => e =>
    {
        if (self.Open == e.Open)
            return Task.CompletedTask;

        self.Open = e.Open;
        return self.OpenChanged.InvokeAsync(e.Open);
    };
}
