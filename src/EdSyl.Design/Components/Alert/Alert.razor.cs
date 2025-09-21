namespace EdSyl.Design.Components;

/// <summary> Alert component. </summary>
/// <remarks> https://flyonui.com/docs/components/alert </remarks>
[ClassName("alert")]
public partial class Alert : SelfRender
{
    [ClassVariantHolder]
    private string? dye = nameof(Primary), variant;

    /// <summary> Children content to include in the element if applicable. </summary>
    [Parameter]
    public override RenderFragment? ChildContent
    {
        get => base.ChildContent ?? field;
        set;
    }

    /// <summary> Gets or sets the title of the alert. </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary> Icon to display in the alert. </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary> Set to prevent showing an icon. </summary>
    [Parameter]
    public bool NoIcon { get; set; }

    /// <summary> Set to allow closing the alert. </summary>
    [Parameter]
    public bool Closable { get; set; }

    /// <summary> Gets or sets the style of the alert. </summary>
    [Parameter]
    public string? Variant
    {
        get => variant;
        set => variant = value;
    }

    /// <summary> Soft style alerts have a subtle background color. </summary>
    /// <remarks> https://flyonui.com/docs/components/alert/#soft-alerts </remarks>
    [Parameter]
    [ClassVariantToggle("alert-soft")]
    public bool Soft
    {
        get => Variance(ref variant);
        set => Variance(ref variant, value);
    }

    /// <summary>  Outline style alerts have a border but transparent background. </summary>
    /// <remarks> https://flyonui.com/docs/components/alert/#outline-alerts </remarks>
    [Parameter]
    [ClassVariantToggle("alert-outline")]
    public bool Outlined
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
    [ClassVariantToggle("alert-primary")]
    public bool Primary
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("alert-secondary")]
    public bool Secondary
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("alert-info")]
    public bool Info
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("alert-success")]
    public bool Success
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("alert-warning")]
    public bool Warning
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    [Parameter]
    [ClassVariantToggle("alert-error")]
    public bool Danger
    {
        get => Variance(ref dye);
        set => Variance(ref dye, value);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Icon = Icon.NullIfEmpty() ?? dye switch
        {
            nameof(Success) => Icons.Success,
            nameof(Danger) => Icons.Error,
            nameof(Warning) => Icons.Disclaimer,
            nameof(Info) => Icons.Info,
            _ => Icons.LightBulb,
        };
    }
}
