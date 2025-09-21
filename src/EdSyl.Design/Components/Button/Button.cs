namespace EdSyl.Design.Components;

/// <summary> HTML button. </summary>
/// <remarks> https://flyonui.com/docs/components/button </remarks>
[ClassName("btn")]
public class Button : Component
{
    private readonly object clickHandler;

    [ClassVariantHolder(Default = nameof(Primary))]
    private string? dye;

    [ClassVariantHolder(Default = nameof(Filled))]
    private string? variant;

    [ClassVariantHolder]
    private string? size, shape;

    private object? onClick;
    private Action? update;
    private Task? job;

    /// <summary> Text to render when no <see cref="ChildContent" /> provided. </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary> Children content to include in the element if applicable. </summary>
    [Parameter]
    public override RenderFragment? ChildContent
    {
        get => base.ChildContent ?? field;
        set;
    }

    /// <summary> Name of the icon to display. </summary>
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    [ClassNameToggle("busy")]
    public bool Busy { get; set; }

    /// <summary> Text to show while processing the async operation. </summary>
    [Parameter]
    public string? BusyText { get; set; }

    /// <summary>
    /// Filled buttons have the most visual impact after the FAB and
    /// should be used for important, final actions that complete a flow.
    /// </summary>
    /// <remarks> https://flyonui.com/docs/components/button/#solid-buttons </remarks>
    /// <remarks> https://m3.material.io/components/buttons/guidelines#c9bcbc0b-ee05-45ad-8e80-e814ae919fbb </remarks>
    [Parameter]
    public bool Filled
    {
        get => VarianceDefault(ref variant);
        set => Variance(ref variant, value);
    }

    /// <summary>
    /// A filled tonal button is an alternative middle ground between filled and outlined buttons.
    /// They’re useful in contexts where a lower-priority button requires slightly more emphasis than an outline would give.
    /// </summary>
    /// <remarks> https://flyonui.com/docs/components/button/#soft-buttons </remarks>
    /// <remarks> https://m3.material.io/components/buttons/guidelines#c9bcbc0b-ee05-45ad-8e80-e814ae919fbb </remarks>
    [Parameter]
    [ClassVariantToggle("btn-soft")]
    public bool Tonal
    {
        get => Variance(ref variant);
        set => Variance(ref variant, value);
    }

    /// <summary>
    /// Outlined buttons are medium-emphasis buttons.
    /// They contain actions that are important, but aren’t the primary action in an app.
    /// </summary>
    /// <remarks> https://flyonui.com/docs/components/button/#outline-buttons </remarks>
    /// <remarks> https://m3.material.io/components/buttons/guidelines#3742b09f-c224-43e0-a83e-541bd29d0f05 </remarks>
    [Parameter]
    [ClassVariantToggle("btn-outline")]
    public bool Outlined
    {
        get => Variance(ref variant);
        set => Variance(ref variant, value);
    }

    /// <summary> Text buttons are used for the lowest priority actions, especially when presenting multiple options. </summary>
    /// <remarks> https://flyonui.com/docs/components/button/#text-buttons </remarks>
    /// <remarks> https://m3.material.io/components/buttons/guidelines#c9bcbc0b-ee05-45ad-8e80-e814ae919fbb </remarks>
    [Parameter]
    [ClassVariantToggle("btn-text")]
    public bool Textual
    {
        get => Variance(ref variant);
        set => Variance(ref variant, value);
    }

    /// <summary> Name of the color variant to use. </summary>
    [Parameter]
    public string? Dye
    {
        get => dye;
        set => dye = value;
    }

    [Parameter]
    [ClassVariantToggle("btn-primary")]
    public bool Primary
    {
        get => VarianceDefault(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-secondary")]
    public bool Secondary
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-accent")]
    public bool Accent
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-info")]
    public bool Info
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-success")]
    public bool Success
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-warning")]
    public bool Warning
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-error")]
    public bool Danger
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-neon")]
    public bool Neon
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-xs")]
    public bool Smaller
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-sm")]
    public bool Small
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-md")]
    public bool Medium
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-lg")]
    public bool Large
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-xl")]
    public bool Larger
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-circle")]
    public bool Circle
    {
        get => Variance(ref shape);
        set => Variance(ref shape, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-square")]
    public bool Square
    {
        get => Variance(ref shape);
        set => Variance(ref shape, value);
    }

    [Parameter]
    [ClassVariantToggle("btn-wide")]
    public bool Wide
    {
        get => Variance(ref shape);
        set => Variance(ref shape, value);
    }

    [Parameter]
    [ClassNameToggle("btn-disabled")]
    public virtual bool Disabled
    {
        get => Busy || field;
        set;
    }

    /// <inheritdoc />
    public Button() => clickHandler = this.Callback<MouseEventArgs>(HandleClick);

    /// <summary> Whether currently processing a request. </summary>
    protected virtual bool IsProcessing => job is { IsCompleted: false };

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Class.InjectTo(Attributes);
        As ??= Attributes.ContainsKey("href") ? "a" : "button";
        if (As == "button") Attributes.TryAdd("type", "button");
        Attributes.Set("role", As != "button" ? "button" : null);
        Attributes.Substitute("onclick", ref onClick, clickHandler);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible) return;

        Class.Render(this);
        Attributes.Toggle("disabled", Disabled);

        builder.OpenElement(0, As ?? "div");
        builder.AddAttribute(1, "disabled", Disabled);
        builder.AddMultipleAttributes(2, Attributes);
        builder.AddElementReferenceCapture(3, Capture);

        if (Busy)
        {
            builder.Icon(10, "svg-spinners--ring-resize");
            if (!string.IsNullOrWhiteSpace(BusyText)) builder.AddContent(15, BusyText);
            else if (ChildContent != null) builder.AddContent(16, ChildContent);
            else if (Text is { Length: > 0 } text) builder.Element(17, "span", text);
        }
        else
        {
            builder.Icon(20, Icon);
            if (ChildContent != null) builder.AddContent(25, ChildContent);
            else if (Text is { Length: > 0 } text) builder.Element(26, "span", text);
        }

        builder.CloseElement();
    }

    protected virtual Task OnClick(MouseEventArgs args)
    {
        return EventCallbacks.InvokeAsync(onClick, args);
    }

    protected void UpdateBusy()
    {
        var busy = IsProcessing;
        if (Busy == busy) return;
        Busy = busy;
        StateHasChanged();
    }

    private Task HandleClick(MouseEventArgs args)
    {
        // wait for the last job to complete
        if (Busy || IsProcessing)
            return Task.CompletedTask;

        // check if runs synchronously
        var task = job = OnClick(args);
        if (task.IsCompleted)
        {
            Complete(task);
            return task;
        }

        // schedule a busy state if taking long
        Browser.SetTimeout(update ??= UpdateBusy, 100);
        return WaitBusy(task);
    }

    private async Task WaitBusy(Task task)
    {
        try { await task; }
        finally { Complete(task); }
    }

    private void Complete(Task task)
    {
        if (task == job) job = null;
        UpdateBusy();
    }
}
