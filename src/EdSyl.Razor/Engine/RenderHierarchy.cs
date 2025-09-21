using System.Runtime.CompilerServices;

namespace EdSyl.Razor;

/// <summary> Allows stacking child content for component inheritance. </summary>
public class RenderHierarchy
{
#if DEBUG
    private readonly List<string> names = [];
#endif
    private readonly List<RenderFragment> fragments;
    private readonly ComponentBase component;
    private int depth;

    /// <summary> Current children to be used in the rendering cycle. </summary>
    public virtual RenderFragment? ChildContent => depth < fragments.Count ? Cycler : null;

    /// <summary> Number of fragments in the hierarchy. </summary>
    protected int Count => fragments.Count;

    /// <summary> Render fragment to be used as child content that cycles through hierarchy. </summary>
    protected RenderFragment Cycler { get; }

    /// <summary> Initializes a new instance of the <see cref="RenderHierarchy" /> class. </summary>
    /// <param name="component">Component to modify to support child content stacking.</param>
    public RenderHierarchy(ComponentBase component)
    {
        ref var render = ref RenderFragment(component);
        this.component = component;
        fragments = [render];
        render = Render;
        Cycler = Cycle;

        Track(0, fragments[0]);
    }

    /// <summary> Add a layer on top of the existing hierarchy to wrap the content with the provided fragment. </summary>
    /// <param name="fragment">Fragment to render as the parent.</param>
    public RenderHierarchy WrapBy(RenderFragment fragment)
    {
        // last is always a final override of BuildRenderTree
        Debug.Assert(component == fragment.Target, "Parent must be the render function of the component itself");
        Debug.Assert(!fragments.Contains(fragment), "Parent already exists in the render hierarchy");
        Track(fragments.Count - 1, fragment);
        fragments.Insert(fragments.Count - 1, fragment);
        return this;
    }

    /// <summary> Add a layer on top of the existing hierarchy to wrap the content with the provided fragment. </summary>
    /// <param name="fragment">Fragment to render as the parent.</param>
    public RenderHierarchy WrapBy(RenderFragment<RenderFragment?> fragment)
    {
        Track(fragments.Count - 1, fragment);
        fragments.Insert(fragments.Count - 1, builder => fragment(MoveNext())(builder));
        return this;
    }

    /// <summary> Add a layer on top of the existing hierarchy to wrap the content with the provided fragment. </summary>
    /// <param name="type">Type of the component to render as the parent.</param>
    public RenderHierarchy WrapBy([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        Track(fragments.Count - 1, type);
        fragments.Insert(fragments.Count - 1, builder =>
        {
            builder.OpenComponent(0, type);
            builder.AddAttribute(1, nameof(ChildContent), MoveNext());
            builder.CloseComponent();
        });
        return this;
    }

    /// <summary> Add a layer on top of the existing hierarchy to wrap the content with the cascading parameter. </summary>
    /// <typeparam name="T">Type of the cascading value.</typeparam>
    /// <param name="value">Value to be provided for children.</param>
    public RenderHierarchy WrapByCascade<T>(T value)
    {
        Track(fragments.Count - 1, typeof(CascadingValue<T>));
        fragments.Insert(fragments.Count - 1, builder => builder.Cascade(value, MoveNext()));
        return this;
    }

    /// <summary> Add a layer on top of the existing hierarchy to wrap the content with the cascading parameter. </summary>
    public RenderHierarchy WrapByCascade()
    {
        Track(fragments.Count - 1, typeof(CascadingValue<>));
        fragments.Insert(fragments.Count - 1, builder => builder.CascadeDynamic(component, MoveNext()));
        return this;
    }

    /// <summary> Move to the next fragment to display as current child content. </summary>
    protected virtual RenderFragment? MoveNext()
    {
        return depth < fragments.Count
            ? fragments[depth++]
            : null;
    }

    private void Render(RenderTreeBuilder builder)
    {
        depth = 1;
        HasNeverRendered(component) = false;
        HasPendingQueuedRender(component) = false;
        fragments[0](builder);
    }

    private void Cycle(RenderTreeBuilder builder)
    {
        if (MoveNext() is { } rf)
            rf(builder);
    }

    [Conditional("DEBUG")]
    private void Track(int index, Delegate rf)
    {
#if DEBUG
        names.Insert(index, $"{rf.Method.DeclaringType}::{rf.Method.Name}");
#endif
    }

    [Conditional("DEBUG")]
    private void Track(int index, Type type)
    {
#if DEBUG
        names.Insert(index, $"{type}::BuildRenderTree");
#endif
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_renderFragment")]
    private static extern ref RenderFragment RenderFragment(ComponentBase component);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_hasNeverRendered")]
    private static extern ref bool HasNeverRendered(ComponentBase component);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_hasPendingQueuedRender")]
    private static extern ref bool HasPendingQueuedRender(ComponentBase component);
}

public class RenderInterceptor(ComponentBase component) : RenderHierarchy(component)
{
    private RenderFragment? input;

    /// <inheritdoc />
    public override RenderFragment? ChildContent => base.ChildContent ?? input;

    /// <summary> Get injection to use as content to enable hierarchy rendering. </summary>
    /// <param name="input">Existing parameter value supplied to the component.</param>
    /// <param name="always">Set to always create an interceptor even if no input supplied.</param>
    public RenderFragment? Intercept(RenderFragment? input, bool always = false)
    {
        // nothing to intercept?
        if (input == null)
        {
            this.input = input;
            return always || Count > 1 ? Cycler : null;
        }

        // already intercepting?
        if (input.Target == this)
            return input;

        // update
        this.input = input;
        return Cycler;
    }

    /// <inheritdoc />
    protected override RenderFragment? MoveNext()
        => base.MoveNext() ?? input;
}

public class RenderInterceptor<T>(ComponentBase component) : RenderHierarchy(component)
{
    private RenderFragment<T>? input;
    private RenderFragment<T>? injection;

    /// <summary> Get injection to use as content to enable hierarchy rendering. </summary>
    /// <param name="input">Existing parameter value supplied to the component.</param>
    /// <param name="always">Set to always create an interceptor even if no input supplied.</param>
    public RenderFragment<T>? Intercept(RenderFragment<T>? input, bool always = false)
    {
        // nothing to intercept?
        if (input == null)
        {
            this.input = input;
            return always || Count > 1 ? injection ??= Injection : null;
        }

        // already intercepting?
        if (input.Target == this)
            return input;

        // update
        this.input = input;
        return injection ??= Injection;
    }

    private RenderFragment Injection(T value)
        => ChildContent
        ?? input?.Invoke(value)
        ?? Templates.Empty;
}
