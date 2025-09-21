using System.Diagnostics.CodeAnalysis;
using Cortex.Net;

namespace EdSyl.MobX;

/// <summary> Bag of reactive properties to use by external classes to easily implement reactivity. </summary>
public class Reactive : IReactiveObject
{
    private readonly Dictionary<string, Atom> atoms = [];

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1033", Justification = "Hidden")]
    ISharedState IReactiveObject.SharedState => MobX.Global;

    /// <summary> Track read access of the property with the given name. </summary>
    /// <param name="name">Name of the property.</param>
    public void TrackRead(string name)
        => Get(name).ReportObserved();

    /// <summary> Track modification of the property with the given name. </summary>
    /// <param name="name">Name of the property.</param>
    public void TrackWrite(string name)
        => Get(name).ReportChanged();

    private Atom Get(string name)
    {
        if (!atoms.TryGetValue(name, out var atom))
            atoms[name] = atom = new() { Name = name };

        return atom;
    }
}
