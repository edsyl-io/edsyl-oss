namespace EdSyl.Design.Components;

public class ToggleEventArgs : EventArgs
{
    /// <summary> Whether the Details HTML Element is currently open. </summary>
    /// <remarks> https://developer.mozilla.org/en-US/docs/Web/HTML/Element/details#open </remarks>
    public bool Open { get; set; }
}
