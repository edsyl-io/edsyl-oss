namespace EdSyl.Design.Components;

public class TooltipMediator : JsMediatorBase
{
    /// <inheritdoc cref="JsMediator.Mount" />
    public bool Mount(ElementReference element, TooltipOptions? options = null)
        => Set(element) && Mediate("Tooltip", options);
}
