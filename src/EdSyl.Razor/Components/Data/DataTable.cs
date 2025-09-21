using EdSyl.Linq;

namespace EdSyl.Razor.Components;

public class DataTable<T> : Component
{
    /// <summary>
    /// Collection of items to render.
    /// </summary>
    [Parameter]
    public virtual IEnumerable<T>? Data { get; set; }

    /// <summary>
    /// Template to use for rendering a list item.
    /// </summary>
    [Parameter]
    public required RenderFragment<T> Template { get; set; }

    /// <summary>
    /// A delegate defining a filtering criteria.
    /// </summary>
    [Parameter]
    public Func<T, bool>? Predicate { get; set; }

    /// <summary> Set to show a loading indicator. </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Template to show while <see cref="IsLoading" />.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; } = DataGroup.DefaultLoadingTemplate;

    /// <summary>
    /// Template to show when provided data is empty.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Template to show when no items matching filtering criteria.
    /// </summary>
    [Parameter]
    public RenderFragment? NoResultsTemplate { get; set; } = DataGroup.DefaultNoResultsTemplate;

    /// <summary>
    /// Template to show before the table.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Template to show after the table.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    /// <summary>
    /// Template to show for a 'thead' element.
    /// </summary>
    [Parameter]
    public RenderFragment? TableHeader { get; set; }

    /// <summary>
    /// Template to show for a 'tfoot' element.
    /// </summary>
    [Parameter]
    public RenderFragment? TableFooter { get; set; }

    /// <summary> Unmatched attributes for the root element. </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    private IEnumerator<T>? iterator;

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsLoading) RenderLoading(builder);
        else if (!Data.TryMoveNext(out iterator)) RenderEmpty(builder);
        else RenderTable(builder);
    }

    private void RenderLoading(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "loading");
        builder.AddMultipleAttributes(2, Attributes);
        builder.AddContent(3, LoadingTemplate);
        builder.CloseElement();
    }

    private void RenderEmpty(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "empty");
        builder.AddMultipleAttributes(2, Attributes);
        builder.AddContent(3, EmptyTemplate);
        builder.CloseElement();
    }

    private void RenderTable(RenderTreeBuilder builder)
    {
        builder.AddContent(0, HeaderTemplate);

        if (iterator!.Seek(Predicate))
        {
            builder.OpenElement(1, "table");
            builder.AddMultipleAttributes(2, Attributes);

            if (TableHeader != null)
            {
                builder.OpenElement(3, "thead");
                builder.AddContent(4, TableHeader);
                builder.CloseElement();
            }

            builder.OpenElement(5, "tbody");
            builder.Region(6, ref iterator, Predicate, Template);
            builder.CloseElement();

            if (TableFooter != null)
            {
                builder.OpenElement(7, "tfoot");
                builder.AddContent(8, TableFooter);
                builder.CloseElement();
            }

            builder.CloseElement();
            builder.AddContent(9, FooterTemplate);
        }
        else
        {
            builder.AddContent(1, NoResultsTemplate);
        }
    }
}
