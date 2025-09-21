using System.Runtime.CompilerServices;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;

namespace EdSyl.Razor.Interop;

public static partial class Js
{
    /// <summary> Get a unique identifier assigned to the provided reference. </summary>
    /// <param name="reference">JS object reference.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Id>k__BackingField")]
    public static extern ref long Id(this JSObjectReference reference);

    /// <summary> Get a unique identifier assigned to the provided reference. </summary>
    /// <param name="reference">JS object reference.</param>
    public static long? Id(this IJSObjectReference reference)
        => reference is JSObjectReference js ? Id(js) : default;

    /// <summary> Check if both element references are equal. </summary>
    /// <param name="a">First element to compare.</param>
    /// <param name="b">Second element to compare.</param>
    public static bool EqualTo(this ElementReference a, ElementReference b)
        => a.Context == b.Context && a.Id == b.Id;

    /// <summary> Removes keyboard focus from the current element. </summary>
    /// <param name="el">HTML element</param>
    /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/blur </remarks>
    public static void Blur(this ElementReference el) => el.Call("blur");

    /// <summary> Sets the value of an attribute on the specified element. </summary>
    /// <param name="el">HTML element</param>
    /// <param name="name">Name of the attribute.</param>
    /// <param name="value">Value to assign to the attribute.</param>
    /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/setAttribute </remarks>
    public static void Attr(this ElementReference el, string name, object value) => el.Call("setAttribute", name, value);

    /// <summary> Scrolls the specified HTML element into the visible area of the browser window. </summary>
    /// <param name="el">HTML element</param>
    /// <param name="options">Optional configuration to control the scroll behavior and alignment.</param>
    /// /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView </remarks>
    public static void ScrollIntoView(this ElementReference el, ScrollIntoViewOptions? options = default) => el.Call("scrollIntoView", options);

    /// <summary> JavaScript scrollIntoView function options. </summary>
    /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView#scrollintoviewoptions </remarks>
    public class ScrollIntoViewOptions
    {
        /// <summary>
        /// Defines the transition animation. Accepts "auto", "instant", or "smooth".
        /// </summary>
        /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView#behavior </remarks>
        public string? Behavior { get; set; } = "auto";

        /// <summary>
        /// Defines vertical alignment. Accepts "start", "center", "end", or "nearest".
        /// </summary>
        /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView#block </remarks>
        public string? Block { get; set; } = "start";

        /// <summary>
        /// Defines horizontal alignment. Accepts "start", "center", "end", or "nearest".
        /// </summary>
        /// <remarks> https://developer.mozilla.org/en-US/docs/Web/API/Element/scrollIntoView#inline </remarks>
        public string? Inline { get; set; } = "nearest";
    }
}
