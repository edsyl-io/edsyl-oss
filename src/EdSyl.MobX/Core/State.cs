using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Cortex.Net;
using Cortex.Net.Core;
using Cortex.Net.Spy;

namespace EdSyl.MobX;

[SuppressMessage("ReSharper", "ConvertToAutoProperty", Justification = "Performance")]
[SuppressMessage("Roslynator", "RCS1085", Justification = "Performance")]
internal sealed class State : ISharedState
{
    /// <inheritdoc />
    public event EventHandler<SpyEventArgs>? SpyEvent
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public event EventHandler<UnhandledExceptionEventArgs>? UnhandledReactionException;

    /// <inheritdoc />
    public int RunId => runId;

    /// <inheritdoc />
    public bool InBatch => batch != 0;

    /// <inheritdoc />
    public bool AllowStateReads => allowStateReads;

    /// <inheritdoc />
    public bool AllowStateChanges => allowStateChanges;

    /// <inheritdoc />
    public IDerivation? TrackingDerivation => trackingDerivation;

    /// <inheritdoc />
    public CortexConfiguration Configuration => configuration;

    /// <inheritdoc />
    public Queue<IObservable> PendingUnobservations => pendingObservables;

    /// <inheritdoc />
    public Queue<Reaction> PendingReactions => pendingReactions;

    /// <inheritdoc />
    public int ComputationDepth { get; set; }

    /// <inheritdoc />
    public bool SuppressReactionErrors { get; set; }

    /// <inheritdoc />
    public int CurrentActionId { get; set; }

    /// <inheritdoc />
    public int NextActionId { get; set; } = 1;

    /// <inheritdoc />
    public IList<IEnhancer> Enhancers { get; } = [];

    /// <inheritdoc />
    public bool ShouldInvoke => configuration is { AutoscheduleActions: true, SynchronizationContext: not null }
                             && configuration.SynchronizationContext != SynchronizationContext.Current;

    private readonly Queue<Reaction> pendingReactions = [];
    private readonly Queue<IObservable> pendingObservables = [];
    private readonly CortexConfiguration configuration = new();

    private int batch;
    private int runId;
    private int uniqueId;
    private bool running;
    private bool allowStateReads;
    private bool allowStateChanges;
    private IDerivation? trackingDerivation;

    /// <inheritdoc />
    public int GetUniqueId() => ++uniqueId;

    /// <inheritdoc />
    public int IncrementRunId() => ++runId;

    /// <inheritdoc />
    public bool StartAllowStateReads(bool allowStateReads)
    {
        var current = this.allowStateReads;
        this.allowStateReads = allowStateReads;
        return current;
    }

    /// <inheritdoc />
    public void EndAllowStateReads(bool previousAllowStateReads)
    {
        allowStateReads = previousAllowStateReads;
    }

    /// <inheritdoc />
    public bool StartAllowStateChanges(bool allowStateChanges)
    {
        var current = this.allowStateChanges;
        this.allowStateChanges = allowStateChanges;
        return current;
    }

    /// <inheritdoc />
    public void EndAllowStateChanges(bool previousAllowStateChanges)
    {
        allowStateChanges = previousAllowStateChanges;
    }

    /// <inheritdoc />
    public IDerivation StartUntracked()
    {
        var current = trackingDerivation;
        trackingDerivation = default;
        return current;
    }

    /// <inheritdoc />
    public IDerivation? StartTracking(IDerivation derivation)
    {
        var current = trackingDerivation;
        trackingDerivation = derivation;
        return current;
    }

    /// <inheritdoc />
    public void EndTracking(IDerivation previousDerivation)
    {
        trackingDerivation = previousDerivation;
    }

    /// <inheritdoc />
    public void StartBatch() => batch++;

    /// <inheritdoc />
    public void EndBatch()
    {
        if (--batch != 0)
            return;

        RunReactions();

        using (Arrays.Rent(pendingObservables, out var observations))
        {
            pendingObservables.Clear();
            EndBatch(observations);
        }
    }

    /// <inheritdoc />
    public void RunReactions()
    {
        if (running || InBatch)
            return;

        running = true;

        try
        {
            var iterations = configuration.MaxReactionIteractions;
            while (pendingReactions.Count > 0)
            {
                if (iterations-- <= 0)
                    throw ReactionDoesNotConverge(iterations, FlushReactions());

                using (Arrays.Rent(pendingReactions, out var reactions))
                {
                    pendingReactions.Clear();
                    Run(reactions);
                }
            }
        }
        finally
        {
            running = false;
        }
    }

    /// <inheritdoc />
    public void OnSpy(object sender, SpyEventArgs spyEventArgs)
    {
        // ignore
    }

    /// <inheritdoc />
    public void OnUnhandledReactionException(Reaction reaction, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
        UnhandledReactionException?.Invoke(this, unhandledExceptionEventArgs);
    }

    private Reaction FlushReactions()
    {
        var reaction = pendingReactions.Peek();
        pendingReactions.Clear();
        return reaction;
    }

    private void Run(ReadOnlySpan<Reaction> reactions)
    {
        foreach (var reaction in reactions)
            Run(reaction);
    }

    private void Run(Reaction reaction)
    {
        if (reaction.IsDisposed)
            return;

        Scheduled(reaction) = false;
        if (!reaction.ShouldCompute())
            return;

        try
        {
            StartBatch();
            Invalidate(reaction)();
        }
        catch (Exception exception)
        {
            ReportExceptionInReaction(reaction, exception);
        }
        finally
        {
            EndBatch();
        }
    }

    private void ReportExceptionInReaction(Reaction reaction, Exception exception)
    {
        if (configuration.DisableErrorBoundaries)
            throw exception;

        OnUnhandledReactionException(reaction, new(exception, false));
    }

    private static void EndBatch(ReadOnlySpan<IObservable> observables)
    {
        foreach (var observable in observables)
        {
            observable.IsPendingUnobservation = false;
            if (observable.HasObservers())
                continue;

            if (observable.IsBeingObserved)
            {
                observable.IsBeingObserved = false;
                observable.OnBecomeUnobserved();
            }

            if (observable is IComputedValue computed)
                computed.Suspend();
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "onInvalidate")]
    private static extern ref Action Invalidate(Reaction reaction);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "isScheduled")]
    private static extern ref bool Scheduled(Reaction reaction);

    private static InvalidOperationException ReactionDoesNotConverge(int iterations, Reaction reaction)
        => new($"Reaction doesn't converge to a stable state after ({iterations}) iterations. Probably there is a cycle in the reactive function: ({reaction.Name})");
}
