namespace EdSyl.Design.Components;

public class Icon : ComponentBase, IStyledComponent
{
    [ClassVariantHolder]
    private string? dye, size;

    /// <inheritdoc />
    public ClassList Class { get; }

    /// <inheritdoc cref="Component.Attributes" />
    [Parameter(CaptureUnmatchedValues = true)]
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public Dictionary<string, object> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary> Name of the icon. </summary>
    /// <remarks> https://icon-sets.iconify.design </remarks>
    [Parameter]
    public string? Name { get; set; }

    /// <summary> Name of the color variant to use. </summary>
    [Parameter]
    public string? Dye
    {
        get => dye;
        set => dye = value;
    }

    [Parameter]
    [ClassVariantToggle("text-primary")]
    public bool Primary
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-secondary")]
    public bool Secondary
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-accent")]
    public bool Accent
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-info")]
    public bool Info
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-success")]
    public bool Success
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-warning")]
    public bool Warning
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("text-error")]
    public bool Danger
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("size-[1em]")]
    public bool Small
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("size-[1.5em]")]
    public bool Medium
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    [Parameter]
    [ClassVariantToggle("size-[2em]")]
    public bool Large
    {
        get => Variance(ref size);
        set => Variance(ref size, value);
    }

    /// <inheritdoc />
    public Icon() => Class = ClassList.For(this);

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        Class.InjectTo(Attributes);
        Class.ToggleVariant("icon", Templates.Iconify(Name));
        Class.Render(this);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrEmpty(Name)) return;
        builder.OpenElement(0, "i");
        builder.AddMultipleAttributes(1, Attributes);
        builder.CloseElement();
    }
}
