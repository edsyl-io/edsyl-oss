using System.Runtime.InteropServices;

namespace EdSyl.Razor;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct RenderContent
{
    private const byte Text = 1;
    private const byte Markup = 2;
    private const byte Fragment = 3;

    private readonly object content;
    private readonly byte type;

    public static RenderContent Of(RenderFragment? fragment, string? text)
    {
        if (fragment != null) return new(fragment, Fragment);
        if (text != null) return new(text, Text);
        return default;
    }

#pragma warning disable CA2225
    public static implicit operator RenderContent(string? text) => text != null ? new(text, Text) : default;
    public static implicit operator RenderContent(MarkupString markup) => new(markup.Value, Markup);
    public static implicit operator RenderContent(RenderFragment? fragment) => fragment != null ? new(fragment, Fragment) : default;
#pragma warning restore CA2225

    private RenderContent(object content, byte type)
    {
        this.content = content;
        this.type = type;
    }

    public void Render(RenderTreeBuilder builder, int sequence)
    {
        switch (type)
        {
            case Text:
                builder.AddContent(sequence, (string)content);
                break;

            case Markup:
                builder.AddMarkupContent(sequence, (string)content);
                break;

            case Fragment:
                builder.AddContent(sequence, (RenderFragment)content);
                break;
        }
    }
}
