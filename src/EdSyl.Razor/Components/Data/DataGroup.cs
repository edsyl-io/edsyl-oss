using EdSyl.Linq;

namespace EdSyl.Razor.Components;

public abstract class DataGroup : Component
{
    /// <summary> Default content to show while loading a data group. </summary>
    public static RenderFragment? DefaultLoadingTemplate { get; set; }

    /// <summary> Default content to show when no data group search yields no results.</summary>
    public static RenderFragment? DefaultNoResultsTemplate { get; set; }

    /// <summary>
    /// Set to show a loading indicator.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Template to show while <see cref="IsLoading" />.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; } = DefaultLoadingTemplate;

    /// <summary>
    /// Template to show when the data source is empty.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Template to show when no items matching filtering criteria.
    /// </summary>
    [Parameter]
    public RenderFragment? NoResultsTemplate { get; set; } = DefaultNoResultsTemplate;

    /// <summary> Template to show before the list. </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary> Placement of the <see cref="HeaderTemplate" /> </summary>
    [Parameter]
    public Placement HeaderPlace { get; set; } = Placement.Outer | Placement.NoResult;

    /// <summary> Template to show after the list. </summary>
    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    /// <summary> Placement of the <see cref="FooterTemplate" /> </summary>
    [Parameter]
    public Placement FooterPlace { get; set; } = Placement.Outer;

    /// <summary> Render fragment for rendering this component. </summary>
    protected RenderFragment Portal { get; }

    /// <summary> Initializes a new instance of the <see cref="DataGroup" /> class. </summary>
    protected DataGroup() => Portal = Render;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascade(this, Portal);

    /// <inheritdoc cref="BuildRenderTree" />
    protected abstract void Render(RenderTreeBuilder builder);

    [Flags]
    public enum Placement : byte
    {
        /// <summary> Display a template inside the main container. </summary>
        [SuppressMessage("Design", "CA1008", Justification = "By Design")]
        Outer = 0,

        /// <summary> Display a template inside the main container. </summary>
        Inner = 1 << 0,

        /// <summary> Display a template whole empty list. </summary>
        Empty = 1 << 1,

        /// <summary> Display a template while no results. </summary>
        NoResult = 1 << 2,
    }

    protected static bool IsInner(Placement place) => (place & Placement.Inner) == Placement.Inner;
    protected static bool IsOuter(Placement place) => (place & Placement.Inner) != Placement.Inner;
    protected static bool WhenEmpty(Placement place) => (place & Placement.Empty) == Placement.Empty;
    protected static bool WhenNoResults(Placement place) => (place & Placement.NoResult) == Placement.NoResult;
}

public abstract class DataGroup<T> : DataGroup, IDataGroup<T>
{
    /// <summary>
    /// A delegate defining a filtering criteria.
    /// </summary>
    [Parameter]
    public virtual Func<T, bool>? Predicate { get; set; }

    /// <summary>
    /// Template to use for rendering a list item.
    /// </summary>
    [Parameter]
    public required RenderFragment<T> Template { get; set; }

    /// <summary>
    /// Enumerable collection of items to render.
    /// </summary>
    protected abstract IEnumerable<T>? View { get; }

    private IEnumerator<T>? iterator;

    /// <inheritdoc />
    public virtual void NotifyItemChanged(T item)
    {
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Cascade(this, Portal);

    /// <inheritdoc />
    protected sealed override void Render(RenderTreeBuilder builder)
    {
        if (IsLoading) RenderLoading(builder);
        else if (!View.TryMoveNext(out iterator)) RenderEmpty(builder);
        else RenderFilled(builder);
    }

    private void RenderLoading(RenderTreeBuilder builder)
    {
        builder.OpenElement(100, "div");
        builder.AddAttribute(101, "data-loading");
        builder.AddMultipleAttributes(102, Attributes);
        builder.AddContent(104, LoadingTemplate);
        builder.CloseElement();

        // element reference available for filled only
        Element = default;
    }

    private void RenderEmpty(RenderTreeBuilder builder)
    {
        // Outer Header
        if (WhenEmpty(HeaderPlace) && IsOuter(HeaderPlace))
            builder.AddContent(200, HeaderTemplate);

        builder.OpenElement(201, As ?? "div");
        builder.AddAttribute(202, "data-empty");
        builder.AddMultipleAttributes(203, Attributes);

        // Inner Header
        if (WhenEmpty(HeaderPlace) && IsInner(HeaderPlace))
            builder.AddContent(205, HeaderTemplate);

        builder.AddContent(206, EmptyTemplate);

        // Inner Footer
        if (WhenEmpty(FooterPlace) && IsInner(FooterPlace))
            builder.AddContent(207, FooterTemplate);

        builder.CloseElement();

        // Outer Footer
        if (WhenEmpty(FooterPlace) && IsOuter(FooterPlace))
            builder.AddContent(208, FooterTemplate);

        // element reference available for filled only
        Element = default;
    }

    private void RenderFilled(RenderTreeBuilder builder)
    {
        if (iterator!.Seek(Predicate))
        {
            // Outer Header
            if (IsOuter(HeaderPlace))
                builder.AddContent(300, HeaderTemplate);

            builder.OpenElement(301, As ?? "div");
            builder.AddAttribute(302, "data-filled");
            builder.AddMultipleAttributes(303, Attributes);
            builder.AddElementReferenceCapture(304, Capture);

            // Inner Header
            if (IsInner(HeaderPlace))
                builder.AddContent(305, HeaderTemplate);

            builder.Region(306, ref iterator, Predicate, Template);

            // Inner Footer
            if (IsInner(FooterPlace))
                builder.AddContent(307, FooterTemplate);

            builder.CloseElement();

            // Outer Footer
            if (IsOuter(FooterPlace))
                builder.AddContent(308, FooterTemplate);
        }
        else
        {
            if (WhenNoResults(HeaderPlace))
                builder.AddContent(300, HeaderTemplate);

            builder.AddContent(401, NoResultsTemplate);

            if (WhenNoResults(FooterPlace))
                builder.AddContent(402, FooterTemplate);
        }
    }
}

public abstract class DataGroup<T, TCollection> : DataGroup<T> where TCollection : IEnumerable<T>
{
    /// <summary>
    /// Collection of items to render.
    /// </summary>
    [Parameter]
    public virtual TCollection? Data { get; set; }

    /// <summary>
    /// Occurs when the collection of items has been modified.
    /// </summary>
    [Parameter]
    public virtual EventCallback<TCollection> DataChanged { get; set; }

    /// <summary> Check if the collection is empty. </summary>
    public virtual bool IsEmpty => Data?.Any() != true;

    /// <inheritdoc />
    protected override IEnumerable<T>? View => Data;
}
