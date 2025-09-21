using System.Diagnostics;
using Cortex.Net;

namespace EdSyl.MobX;

public abstract class Observable : IObservable
{
    /// <inheritdoc />
    public event EventHandler? BecomeObserved
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public event EventHandler? BecomeUnobserved
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public string Name { get; set; } = default!;

    /// <inheritdoc />
    public ISharedState SharedState => MobX.Global;

    /// <inheritdoc />
    public ISet<IDerivation> Observers { get; } = new HashSet<IDerivation>();

    /// <inheritdoc />
    public bool IsPendingUnobservation { get; set; }

    /// <inheritdoc />
    public int LastAccessedBy { get; set; }

    /// <inheritdoc />
    public bool IsBeingObserved { get; set; }

    /// <inheritdoc />
    public DerivationState LowestObserverState { get; set; } = DerivationState.NotTracking;

    /// <inheritdoc />
    public void OnBecomeObserved() { }

    /// <inheritdoc />
    public void OnBecomeUnobserved() { }

    [Conditional("DEBUG")]
    protected void CheckIfStateModificationsAreAllowed()
    {
        var state = SharedState;

        // should never be possible to change an observed observable from inside computed, see Mobx #798
        if (state.ComputationDepth > 0 && Observers.Count > 0)
            throw ComputedValuesAreNotAllowedToCauseSideEffects(Name);

        // should not be possible to change the observed state outside strict mode, except during initialization, see Mobx #563
        var configuration = state.Configuration;
        if (!state.AllowStateChanges && configuration.EnforceActions != EnforceAction.Never && (Observers.Count > 0 || configuration.EnforceActions == EnforceAction.Always))
            throw configuration.EnforceActions == EnforceAction.Always
                ? ModifiedOutsideActionEnforceAlways(Name)
                : ModifiedOutsideAction(Name);
    }

    private static InvalidOperationException ComputedValuesAreNotAllowedToCauseSideEffects(string name)
        => new($"Computed values are not allowed to cause side effects by changing observables that are already being observed. Tried to modify: ({name})");

    private static InvalidOperationException ModifiedOutsideActionEnforceAlways(string name)
        => new($"Since strict-mode is enabled, changing observed observable values outside actions is not allowed. Please wrap the code in an `action` if this change is intended. Tried to modify: ({name})");

    private static InvalidOperationException ModifiedOutsideAction(string name)
        => new($"Side effects like changing state are not allowed at this point. Are you trying to modify state from, for example, the render function of a React component? Tried to modify: ({name})");
}
