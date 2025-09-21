using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cortex.Net;

namespace EdSyl.MobX;

/// <summary> Base class for objects to hold reactive properties. </summary>
/// <remarks>https://mobx.js.org/defining-data-stores.html#stores</remarks>
public abstract class Store : IReactiveObject
{
    private readonly Dictionary<string, IValue> bag = [];

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1033", Justification = "Hidden")]
    ISharedState IReactiveObject.SharedState => MobX.Global;

    /// <inheritdoc cref="Cortex.Net.Types.ObservableObject.Read{T}(string)" />
    protected T Read<T>([CallerMemberName] string? name = default)
    {
        // read existing property if any
        if (bag.TryGetValue(name!, out var existing))
            return ((IValue<T>)existing).Value;

        // allocate property and track reading
        var box = new Box<T> { Name = name! };
        bag[name!] = box;
        return box;
    }

    /// <inheritdoc cref="Cortex.Net.Types.ObservableObject.Write{T}(string,T)" />
    protected void Write<T>(T value, [CallerMemberName] string? name = default)
    {
        if (bag.TryGetValue(name!, out var existing))
            ((IValue<T>)existing).Value = value;
        else
            bag[name!] = new Box<T>(value) { Name = name! };
    }
}
