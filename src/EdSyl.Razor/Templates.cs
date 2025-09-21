using System.Runtime.CompilerServices;
using EdSyl.Linq;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.Razor;

public static class Templates
{
    /// <summary> A render fragment doing nothing. </summary>
    public static readonly RenderFragment Empty = _ => { };

    /// <inheritdoc cref="ComponentBase.StateHasChanged" />
    /// <param name="component">A component to notify.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(StateHasChanged))]
    public static extern void StateHasChanged(this ComponentBase component);

    /// <summary> Create a weakly typed fragment. </summary>
    /// <param name="fragment">Strongly-typed fragment.</param>
    /// <typeparam name="T">Type of the input parameter.</typeparam>
    /// <returns>A weakly typed fragment.</returns>
    public static RenderFragment<object>? Weaken<T>(RenderFragment<T>? fragment)
    {
        return fragment != null
            ? v => fragment((T)v)
            : null;
    }

    /// <summary> Create a markup fragment. </summary>
    /// <param name="markup">Raw HTML markup to render</param>
    public static RenderFragment? Markup(string? markup) => !string.IsNullOrWhiteSpace(markup)
        ? builder => builder.AddMarkupContent(0, markup)
        : null;

    /// <summary> Render a component without parameters. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="component">Type of the component to render.</param>
    public static void Component(this RenderTreeBuilder builder, int sequence, [DynamicallyAccessedMembers(All)] Type component)
    {
        builder.OpenComponent(sequence, component);
        builder.CloseComponent();
    }

    /// <summary> Render a component without parameters. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="component">Type of the component to render.</param>
    /// <param name="attributes">Component parameters.</param>
    [SuppressMessage("Design", "MA0016", Justification = "Syntactic sugar")]
    public static int Component(this RenderTreeBuilder builder, int sequence, [DynamicallyAccessedMembers(All)] Type component, Dictionary<string, object> attributes)
    {
        builder.OpenComponent(sequence++, component);
        builder.AddMultipleAttributes(sequence++, attributes);
        builder.CloseComponent();
        return sequence;
    }

    /// <summary> Render HTML element. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="tag">HTML tag to render.</param>
    /// <param name="content">Content to render.</param>
    public static int Element(this RenderTreeBuilder builder, int sequence, string? tag, object? content)
    {
        builder.OpenElement(sequence++, tag ?? "div");
        builder.AddContent(sequence++, content);
        builder.CloseElement();
        return sequence;
    }

    /// <summary> Render HTML element. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="tag">HTML tag to render.</param>
    /// <param name="attributes">HTML attributes to include.</param>
    public static void Element(this RenderTreeBuilder builder, string? tag, IEnumerable<KeyValuePair<string, object>>? attributes)
    {
        builder.OpenElement(0, tag ?? "div");
        builder.AddMultipleAttributes(1, attributes);
        builder.CloseElement();
    }

    /// <summary> Render HTML element. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="tag">HTML tag to render.</param>
    /// <param name="attributes">HTML attributes to include.</param>
    /// <param name="content">Content to render.</param>
    public static void Element(this RenderTreeBuilder builder, string? tag, IEnumerable<KeyValuePair<string, object>>? attributes, object? content)
    {
        builder.OpenElement(0, tag ?? "div");
        builder.AddMultipleAttributes(1, attributes);
        builder.AddContent(2, content);
        builder.CloseElement();
    }

    /// <summary> Render HTML element. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="tag">HTML tag to render.</param>
    /// <param name="attributes">HTML attributes to include.</param>
    /// <param name="fragment">Content to render.</param>
    public static void Element(this RenderTreeBuilder builder, string? tag, IEnumerable<KeyValuePair<string, object>>? attributes, RenderFragment? fragment)
    {
        builder.OpenElement(0, tag ?? "div");
        builder.AddMultipleAttributes(1, attributes);
        builder.AddContent(2, fragment);
        builder.CloseElement();
    }

    /// <summary> Render HTML element. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="tag">HTML tag to render.</param>
    /// <param name="attributes">HTML attributes to include.</param>
    /// <param name="fragment">Content to render.</param>
    /// <param name="fallback">Fallback text to use if the fragment is null.</param>
    public static void Element(this RenderTreeBuilder builder, string? tag, IEnumerable<KeyValuePair<string, object>>? attributes, RenderFragment? fragment, string? fallback)
    {
        builder.OpenElement(0, tag ?? "div");
        builder.AddMultipleAttributes(1, attributes);
        Content(builder, 2, fragment, fallback);
        builder.CloseElement();
    }

    /// <summary> Render an icon with the particular name. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="name">Name of the icon.</param>
    public static int Icon(this RenderTreeBuilder builder, int sequence, string? name)
    {
        if (name == null) return sequence;
        builder.OpenElement(sequence++, "i");
        builder.AddAttribute(sequence++, "class", Iconify(name));
        builder.CloseElement();
        return sequence;
    }

    [Obsolete("Use icon from the iconify library directly", false)]
    public static string? Iconify([MaybeNullWhen(true)] string? name) => name switch
    {
        "add" => "material-symbols--add",
        "add_notes" => "material-symbols--add-notes",
        "article_shortcut" => "material-symbols--article-shortcut",
        "assignment" => "material-symbols--assignment",
        "attach_file_add" => "material-symbols--attach-file-add",
        "auto_awesome" => "material-symbols--auto-awesome",
        "auto_fix_high" => "mdi--magic",
        "ballot" => "material-symbols--ballot",
        "check" => "material-symbols--check",
        "check_box" => "material-symbols--check-box",
        "check_circle_outline" => "material-symbols--check-circle-outline",
        "checklist" => "material-symbols--checklist",
        "chevron_backward" => "material-symbols--chevron-backward",
        "chevron_right" => "material-symbols--chevron-right",
        "close" => "material-symbols--close",
        "content_copy" => "material-symbols--content-copy",
        "delete" => "material-symbols--delete-outline",
        "description" => "material-symbols--description",
        "edit" => "material-symbols--edit-outline",
        "edit_note" => "material-symbols--edit-note",
        "emoji_events" => "material-symbols--emoji-events",
        "emoji_objects" => "material-symbols--emoji-objects-outline",
        "error_outline" => "material-symbols--error-outline",
        "file_upload" => "material-symbols--file-upload",
        "folder" => "material-symbols--folder-outline",
        "format_list_bulleted" => "material-symbols--format-list-bulleted",
        "fullscreen" => "material-symbols--fullscreen",
        "fullscreen_exit" => "material-symbols--fullscreen-exit",
        "group" => "material-symbols--group-outline",
        "help" => "material-symbols--help-outline",
        "history" => "material-symbols--history",
        "iframe" => "material-symbols--iframe",
        "info" => "material-symbols--info",
        "info_outline" => "material-symbols--info-outline",
        "keyboard_arrow_down" => "material-symbols--keyboard-arrow-down",
        "lightbulb_outline" => "material-symbols--lightbulb-outline",
        "link" => "material-symbols--link",
        "link_off" => "material-symbols--link-off",
        "location_city" => "material-symbols--location-city",
        "more_horiz" => "material-symbols--more-horiz",
        "more_vert" => "material-symbols--more-vert",
        "navigate_next" => "material-symbols--navigate-next",
        "open_in_new" => "material-symbols--open-in-new",
        "palette" => "material-symbols--palette",
        "pending_actions" => "material-symbols--pending-actions",
        "preview" => "material-symbols--preview",
        "print" => "material-symbols--print-outline",
        "psychology" => "material-symbols--psychology",
        "question_mark" => "material-symbols--question-mark",
        "remove" => "material-symbols--remove",
        "rocket_launch" => "material-symbols--rocket-launch",
        "rule" => "material-symbols--rule",
        "save_as" => "material-symbols--save-as",
        "school" => "material-symbols--school-outline",
        "search" => "material-symbols--search",
        "send" => "material-symbols--send",
        "settings" => "material-symbols--settings",
        "task_alt" => "material-symbols--task-alt",
        "thumbs_up_down" => "material-symbols--thumbs-up-down-outline",
        "tips_and_updates" => "material-symbols--tips-and-updates",
        "topic" => "material-symbols--topic-outline",
        "translate" => "material-symbols--translate",
        "tune" => "material-symbols--tune",
        "undo" => "material-symbols--undo",
        "upload_file" => "material-symbols--upload-file",
        "warning" => "material-symbols--warning",
        "warning_amber" => "material-symbols--warning-amber",
        "work_history" => "material-symbols--work-history-outline",
        _ => name,
    };

    /// <summary> Render a fragment or fallback to a text content. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="fragment">Content to render.</param>
    /// <param name="fallback">Fallback text to use if the fragment is null.</param>
    public static int Content(this RenderTreeBuilder builder, int sequence, RenderFragment? fragment, string? fallback)
    {
        if (fragment != null) builder.AddContent(sequence, fragment);
        else if (fallback != null) builder.AddContent(sequence, fallback);
        return sequence;
    }

    /// <summary> Render a fragment or fallback to a text content. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="fragment">Content to render.</param>
    /// <param name="tag">HTML tag for the fallback text.</param>
    /// <param name="fallback">Fallback text to use if the fragment is null.</param>
    public static int Content(this RenderTreeBuilder builder, int sequence, RenderFragment? fragment, string? tag, string? fallback)
    {
        return fragment != null || tag == null
            ? Content(builder, sequence, fragment, fallback)
            : Element(builder, sequence, tag, fallback);
    }

    /// <summary>Appends a frame representing markup content.</summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="markup">Content for the new markup frame.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Markup(this RenderTreeBuilder builder, int sequence, [LanguageInjection(InjectedLanguage.HTML)] string? markup)
        => builder.AddMarkupContent(sequence, markup);

    /// <summary> Render a class name attribute. </summary>
    /// <param name="builder">Renderer to receive the attribute.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="klass">Value to use for the class name.</param>
    public static void ClassName(this RenderTreeBuilder builder, int sequence, [LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? klass)
    {
        builder.AddAttribute(sequence, "class", klass);
    }

    /// <summary> Render a list of fragments in a single region. </summary>
    /// <param name="fragments">List of fragments to render.</param>
    /// <returns>A fragment rendering the given list of fragments if any; null otherwise.</returns>
    public static RenderFragment? Region(params RenderFragment?[] fragments)
        => Any(fragments) ? builder => Region(builder, fragments) : null;

    /// <summary> Render a list of items inside a region of frames.</summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="source">A collection of items to display.</param>
    /// <param name="template">Template to apply for each item.</param>
    /// <typeparam name="T">Type of the items.</typeparam>
    /// <returns>The number of items iterated.</returns>
    public static int Region<T>(this RenderTreeBuilder builder, int sequence, IEnumerable<T>? source, RenderFragment<T> template)
    {
        return source.TryMoveNext(out var iterator)
            ? Region(builder, 0, ref iterator, template)
            : sequence;
    }

    /// <summary> Render a list of items inside a region of frames.</summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="iterator">An initialized enumerator with the current element set to the first element.</param>
    /// <param name="template">Template to apply for each item.</param>
    /// <typeparam name="T">Type of the items.</typeparam>
    /// <returns>The number of items iterated.</returns>
    public static int Region<T>(this RenderTreeBuilder builder, int sequence, ref IEnumerator<T>? iterator, RenderFragment<T> template)
    {
        var count = 0;
        builder.OpenRegion(sequence);

        do
        {
            template(iterator!.Current)(builder);
            count++;
        } while (iterator.MoveNext());

        builder.CloseRegion();
        iterator.Dispose();
        iterator = default!;
        return count;
    }

    /// <summary> Render a list of items inside a region of frames that match a given condition.</summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">An integer that represents the position of the instruction in the source code.</param>
    /// <param name="iterator">An initialized enumerator with the current element set to the first element.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="template">Template to apply for each item.</param>
    /// <typeparam name="T">Type of the items.</typeparam>
    /// <returns>Number of items matching the given condition.</returns>
    public static int Region<T>(this RenderTreeBuilder builder, int sequence, ref IEnumerator<T>? iterator, Func<T, bool>? predicate, RenderFragment<T> template)
    {
        if (predicate == null)
            return Region(builder, sequence, ref iterator, template);

        var count = 0;
        builder.OpenRegion(sequence);

        do
        {
            var current = iterator!.Current;
            if (!predicate(current)) continue;
            template(current)(builder);
            count++;
        } while (iterator.MoveNext());

        builder.CloseRegion();
        iterator.Dispose();
        iterator = default!;
        return count;
    }

    /// <summary> Render a fragment within a <see cref="CascadingValue{TValue}" />. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="sequence">Position of the instruction in the source code.</param>
    /// <param name="value">Value to be provided for children.</param>
    /// <param name="rf">The content to which the value should be provided.</param>
    /// <typeparam name="T">Type of the cascading value.</typeparam>
    public static void Cascade<T>(this RenderTreeBuilder builder, int sequence, T value, RenderFragment? rf)
    {
        builder.OpenComponent<CascadingValue<T>>(sequence);
        builder.AddAttribute(sequence + 1, nameof(CascadingValue<>.IsFixed), true);
        builder.AddAttribute(sequence + 2, nameof(CascadingValue<>.Value), value);
        builder.AddAttribute(sequence + 3, nameof(CascadingValue<>.ChildContent), rf);
        builder.CloseComponent();
    }

    /// <summary> Render a fragment within a <see cref="CascadingValue{TValue}" />. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="value">Value to be provided for children.</param>
    /// <param name="rf">The content to which the value should be provided.</param>
    /// <typeparam name="T">Type of the cascading value.</typeparam>
    public static void Cascade<T>(this RenderTreeBuilder builder, T value, RenderFragment? rf)
    {
        builder.OpenComponent<CascadingValue<T>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<>.IsFixed), true);
        builder.AddAttribute(2, nameof(CascadingValue<>.Value), value);
        builder.AddAttribute(3, nameof(CascadingValue<>.ChildContent), rf);
        builder.CloseComponent();
    }

    /// <summary> Render a fragment within a <see cref="CascadingValue{TValue}" />. </summary>
    /// <param name="builder">Renderer to receive the content.</param>
    /// <param name="value">Value to be provided for children.</param>
    /// <param name="rf">The content to which the value should be provided.</param>
    public static void CascadeDynamic(this RenderTreeBuilder builder, object value, RenderFragment? rf)
    {
        builder.OpenComponent(0, Cache.GetCascadeComponentType(value.GetType()));
        builder.AddAttribute(1, nameof(CascadingValue<>.IsFixed), true);
        builder.AddAttribute(2, nameof(CascadingValue<>.Value), value);
        builder.AddAttribute(3, nameof(CascadingValue<>.ChildContent), rf);
        builder.CloseComponent();
    }

    private static void Region(RenderTreeBuilder builder, RenderFragment?[] fragments)
    {
        builder.OpenRegion(0);

        foreach (var fragment in fragments)
            fragment?.Invoke(builder);

        builder.CloseRegion();
    }

    private static bool Any(RenderFragment?[] fragments)
    {
        foreach (var fragment in fragments)
            if (fragment != null)
                return true;

        return false;
    }
}

[SuppressMessage("Design", "MA0018", Justification = "Caching")]
public static class Templates<[DynamicallyAccessedMembers(All)] T>
{
    public static readonly RenderFragment Fragment = builder => builder.Component(0, typeof(T));
}

file static class Cache
{
    private static readonly Dictionary<Type, Type> Cascades = [];

    [return: DynamicallyAccessedMembers(All)]
    public static Type GetCascadeComponentType(Type valueType)
    {
        if (!Cascades.TryGetValue(valueType, out var cascade))
            Cascades[valueType] = cascade = typeof(CascadingValue<>).MakeGenericType(valueType);

        return cascade;
    }
}
