using System.Runtime.CompilerServices;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace EdSyl.Razor.Interop;

public static partial class Js
{
    /// <summary> Get JavaScript in-process runtime of the provided element. </summary>
    /// <param name="element">Element holding a runtime.</param>
    public static IJSRuntime? JsRuntime(this ElementReference element) => element.Context?.JsRuntime();

    /// <summary> Get JavaScript in-process runtime of the provided context. </summary>
    /// <param name="context">Context holding element references.</param>
    public static IJSRuntime? JsRuntime(this ElementReferenceContext context) => context is WebElementReferenceContext web ? JsRuntime(web) : default;

    /// <summary> Get JavaScript runtime of the provided element. </summary>
    /// <param name="reference">Reference holding a runtime.</param>
    public static IJSRuntime? JsRuntime(this IJSObjectReference reference) => reference is JSObjectReference js ? JsRuntime(js) : default;

    /// <summary> Get JavaScript in-process runtime of the provided context. </summary>
    /// <param name="context">Context holding element references.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<JSRuntime>k__BackingField")]
    public static extern ref IJSRuntime JsRuntime(this WebElementReferenceContext context);

    /// <summary> Get JavaScript runtime of the provided element. </summary>
    /// <param name="reference">Reference holding a runtime.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_jsRuntime")]
    public static extern ref JSRuntime JsRuntime(this JSObjectReference reference);

    private static InvalidOperationException NoRuntime() => new("Unable to extract JS runtime");
}
