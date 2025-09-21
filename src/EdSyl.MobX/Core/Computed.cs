using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cortex.Net;
using Cortex.Net.Core;
using Cortex.Net.Types;

namespace EdSyl.MobX;

public class Computed : Observable, IDerivation
{
    /// <inheritdoc />
    public int RunId { get; set; }

    /// <inheritdoc />
    public DerivationState DependenciesState { get; set; } = DerivationState.NotTracking;

    /// <inheritdoc />
    public ISet<IObservable> Observing { get; } = new HashSet<IObservable>();

    /// <inheritdoc />
    public ISet<IObservable> NewObserving { get; } = new HashSet<IObservable>();

    /// <summary> Set to keep value calculating, even when it is not observed. </summary>
    public bool KeepAlive { get; set; }

    /// <inheritdoc />
    public bool RequiresObservable => false;

    /// <summary> Set to indicate that computed value can't be accessed outside the reactive context. </summary>
    public bool RequiresReaction { get; set; }

    /// <inheritdoc />
    public TraceMode IsTracing
    {
        get => TraceMode.None;
        set => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public void OnBecomeStale() => this.PropagateMaybeChanged();

    [Conditional("DEBUG")]
    protected void WarnUntrackedRead()
    {
        if (RequiresReaction)
            throw ReadOutsideReaction();

        if (SharedState.Configuration.ComputedRequiresReaction)
            Console.WriteLine("Read outside a reactive context. Doing full recompute");
    }

    protected static InvalidOperationException CyclicComputation() => new("Cyclic computation");
    private static InvalidOperationException ReadOutsideReaction() => new("Read outside a reactive context");
}

public sealed class Computed<T>(Func<T> getter) : Computed, IComputedValue<T>
{
    [SuppressMessage("Usage", "CA2225", Justification = "Syntactic Sugar")]
    public static implicit operator T(Computed<T> box) => box.Value;

    /// <inheritdoc />
    public event EventHandler<ValueChangedEventArgs<T>>? Changed
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    private T? value;
    private byte cycle;
    private Exception? error;

    /// <inheritdoc />
    public T Value
    {
        get => Get();
        set => throw new NotSupportedException("setter is not supported");
    }

    /// <inheritdoc />
    object IValue.Value
    {
        get => Get()!;
        set => throw new NotSupportedException("setter is not supported");
    }

    /// <inheritdoc />
    public void Suspend()
    {
        if (KeepAlive) return;
        this.ClearObserving();
        value = default!;
        error = default;
    }

    /// <inheritdoc />
    IDisposable IComputedValue<T>.Observe(EventHandler<ValueChangedEventArgs<T>> changedEventHandler, bool fireImmediately)
        => throw new NotSupportedException();

    private T Get()
    {
        switch (cycle)
        {
            // allow 1 level of reentrancy by calling the derivation again
            case 1:
                cycle++;
                return value = getter();
            case > 2:
                throw CyclicComputation();
        }

        if (KeepAlive || SharedState.InBatch || this.HasObservers())
        {
            this.ReportObserved();
            if (this.ShouldCompute())
                Recalculate();
        }
        else if (this.ShouldCompute())
        {
            WarnUntrackedRead();
            SharedState.StartBatch();
            Compute(track: false);
            SharedState.EndBatch();
        }

        // rethrow exception within own boundary
        if (error != null)
            throw error;

        return value!;
    }

    private void Recalculate()
    {
        var last = value;
        var untracked = DependenciesState == DerivationState.NotTracking;
        Compute(track: true);

        if (untracked || Differs(last, value))
            this.PropagateChangeConfirmed();
    }

    private void Compute(bool track)
    {
        cycle = 1;
        error = default;
        SharedState.ComputationDepth++;

        if (track)
        {
            (value, error) = this.TrackDerivedFunction(getter);
        }
        else
        {
            try { value = getter(); }
            catch (Exception e) { error = e; }
        }

        // cleanup
        cycle = 0;
        SharedState.ComputationDepth--;

        // propagate error immediately
        if (error != null && SharedState.Configuration.DisableErrorBoundaries)
            throw error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Differs(T? a, T? b)
        => !EqualityComparer<T>.Default.Equals(a, b);
}
