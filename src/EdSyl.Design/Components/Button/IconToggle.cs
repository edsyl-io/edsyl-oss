namespace EdSyl.Design.Components;

public class IconToggle : Button
{
    /// <summary> Whether the toggle is checked. </summary>
    [Parameter]
    public bool Checked { get; set; }

    /// <summary> Occurs when <see cref="Checked" /> changes. </summary>
    [Parameter]
    public EventCallback<bool> CheckedChanged { get; set; }

    /// <summary> Icon to display when <see cref="Checked" /> is true. </summary>
    [Parameter]
    public required string CheckedIcon { get; set; }

    /// <summary> Icon to display when <see cref="Checked" /> is false. </summary>
    [Parameter]
    public required string UncheckedIcon { get; set; }

    /// <inheritdoc />
    protected override async Task OnClick(MouseEventArgs args)
    {
        await base.OnClick(args);
        await Toggle();
        Element.Call("blur");
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Icon = Checked ? CheckedIcon : UncheckedIcon;
        base.BuildRenderTree(builder);
    }

    private async Task Toggle()
    {
        Checked = !Checked;
        await CheckedChanged.InvokeAsync(Checked);
    }
}
