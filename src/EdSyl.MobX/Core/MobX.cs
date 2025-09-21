using System.Diagnostics.CodeAnalysis;
using Cortex.Net;
using Cortex.Net.Blazor;
using Microsoft.AspNetCore.Components.Rendering;

namespace EdSyl.MobX;

[SuppressMessage("Design", "MA0049", Justification = "By Design")]
public static class MobX
{
    /// <summary> Default global state. </summary>
    internal static readonly State Global = new();

    /// <summary> An action is any piece of code that modifies the state. </summary>
    /// <remarks>https://mobx.js.org/api.html#actions</remarks>
    public static Transaction Action() => new(Global);

    /// <summary> Create an observer to automatically redraw a component when any of the dependencies change. </summary>
    /// <param name="name">Name of the component.</param>
    /// <param name="rf">Render fragment to render.</param>
    /// <param name="effect">Side effect to run when any dependency changes.</param>
    public static ObserverObject Observer(string name, Action<RenderTreeBuilder> rf, Action effect)
        => new(Global, name, rf, effect);

    /// <summary> Apply a global configuration. </summary>
    /// <param name="configure">Action to configure global state.</param>
    public static void Configure(Action<CortexConfiguration> configure)
        => configure(Global.Configuration);
}
