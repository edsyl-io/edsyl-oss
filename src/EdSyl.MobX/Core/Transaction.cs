using System.Diagnostics.CodeAnalysis;
using Cortex.Net;

namespace EdSyl.MobX;

/// <summary> Batch a bunch of updates without running any reactions until the end of the transaction. </summary>
/// <remarks>https://mobx.js.org/api.html#actions</remarks>
[SuppressMessage("Performance", "CA1815", Justification = "Unnecessary")]
public readonly struct Transaction : IDisposable
{
    private readonly int parent;
    private readonly bool allowStateReads;
    private readonly bool allowStateChanges;
    private readonly IDerivation derivation;
    private readonly ISharedState state;

    public Transaction(ISharedState state)
    {
        this.state = state;
        parent = state.CurrentActionId;
        derivation = state.StartUntracked();
        allowStateChanges = state.StartAllowStateChanges(true);
        allowStateReads = state.StartAllowStateReads(true);
        state.CurrentActionId = state.NextActionId++;
        state.StartBatch();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        state.CurrentActionId = parent;
        state.EndAllowStateChanges(allowStateChanges);
        state.EndAllowStateReads(allowStateReads);
        state.EndBatch();
        state.EndTracking(derivation);
        state.SuppressReactionErrors = false;
    }
}
