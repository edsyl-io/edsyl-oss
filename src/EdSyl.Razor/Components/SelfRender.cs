namespace EdSyl.Razor.Components;

/// <summary> Base class for components that render itself as a child content. </summary>
public class SelfRender : Component
{
    /// <inheritdoc />
    public SelfRender() => WrapBy(base.BuildRenderTree);
}

/// <summary> Base class for components that render itself as a child content, yet allowing to pass children. </summary>
public class SelfRenderWithChildren : ComponentWithChildren
{
    /// <inheritdoc />
    public SelfRenderWithChildren() => WrapBy(base.BuildRenderTree);
}
