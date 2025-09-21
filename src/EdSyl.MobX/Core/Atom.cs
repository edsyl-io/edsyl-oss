using System.Runtime.CompilerServices;
using Cortex.Net;
using Cortex.Net.Core;

namespace EdSyl.MobX;

/// <summary> Base class for any reactive object. </summary>
public class Atom : Observable, IAtom
{
    /// <inheritdoc />
    public bool ReportObserved()
        => ObservableExtensions.ReportObserved(this);

    /// <inheritdoc />
    public void ReportChanged()
    {
        SharedState.StartBatch();
        this.PropagateChanged();
        SharedState.EndBatch();
    }

    /// <summary> Record use of the atom before returning a value. </summary>
    /// <param name="value">Value to return.</param>
    /// <typeparam name="T">Type of the value.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected T Access<T>(T value)
    {
        ReportObserved();
        return value;
    }
}
