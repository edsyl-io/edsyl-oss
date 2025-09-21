namespace EdSyl.Design.Components;

public class FullScreenToggle : IconToggle
{
    public FullScreenToggle()
    {
        Textual = true;
        Secondary = true;
        CheckedIcon = Icons.FullScreenOff;
        UncheckedIcon = Icons.FullScreenOn;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Attributes.TryAdd("title", "Toggle Full Screen");
    }
}
