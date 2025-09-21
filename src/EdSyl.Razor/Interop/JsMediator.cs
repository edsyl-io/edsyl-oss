using Microsoft.JSInterop;
using static EdSyl.Razor.Interop.Js;

namespace EdSyl.Razor.Interop;

/// <inheritdoc />
public abstract class JsMediator : JsMediatorBase
{
    /// <summary> Mount mediator to the given HTML element. Does nothing if element reference doesn't change. </summary>
    /// <param name="element">Element to create a mediator for.</param>
    /// <returns>True when new mediator created; false otherwise.</returns>
    public abstract bool Mount(ElementReference element);
}

/// <summary> Encapsulates interop with a JS object mounted on a specific HTML element. </summary>
public abstract class JsMediatorBase : IDisposable
{
    private object? js;
    private ElementReference el;

    /// <summary> Indicates if the mediator is ready for use. </summary>
    public bool Ready => js != null;

    /// <inheritdoc />
    public virtual void Dispose()
    {
        DisposeJsRef(ref js);
        el = default;
    }

    /// <summary> Reference to a currently available JavaScript object or one being initializing. </summary>
    protected ValueTask<IJSObjectReference> Js => ReadJsRef(js!);

    /// <summary> Setup mediator for provided element reference. </summary>
    /// <param name="element">Element reference to use.</param>
    /// <param name="name">Name of the mediator to mount.</param>
    protected bool Setup(ElementReference element, string name)
        => Set(element) && Mediate(name);

    /// <summary> Set an element reference to a given value. </summary>
    /// <param name="element">Element reference to use.</param>
    /// <returns>True when the value differs; false otherwise.</returns>
    protected bool Set(ElementReference element)
    {
        if (el.EqualTo(element))
            return false;

        DisposeJsRef(ref js);
        el = element;
        return true;
    }

    /// <summary> Mount a JS mediator by the given name. </summary>
    /// <param name="name">Name of the mediator to mount.</param>
    /// <param name="args">Parameters to supply for mediator constructor.</param>
    /// <returns>True on success; false otherwise.</returns>
    protected bool Mediate(string name, params object?[]? args)
    {
        if (el.Context == null) return false;
        SetJsRef(this, el.Mediate(name, args), (self, x) => self.js = x);
        return true;
    }
}

public static partial class Js
{
    private const string MediateFn = "edsyl.mediate";

    /// <summary> Mount mediator to the provided HTML element. </summary>
    /// <param name="element">Element to hold the mediator.</param>
    /// <param name="mediator">Name of the mediator.</param>
    /// <param name="args">JSON-serializable arguments to supply for mediator constructor.</param>
    /// <exception cref="InvalidOperationException">When no JS runtime available.</exception>
    public static ValueTask<IJSObjectReference> Mediate(this ElementReference element, string mediator, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).Mediate(element, mediator, args);

    /// <summary> Mount mediator to the provided HTML element. </summary>
    /// <param name="runtime">Reference to a JavaScript runtime.</param>
    /// <param name="element">Element to attach mediator for.</param>
    /// <param name="mediator">Name of the mediator.</param>
    /// <param name="args">JSON-serializable arguments to supply for mediator constructor.</param>
    public static ValueTask<IJSObjectReference> Mediate(this IJSRuntime runtime, ElementReference element, string mediator, params object?[]? args)
        => runtime.Execute<IJSObjectReference>(MediateFn, element, mediator, args);
}
