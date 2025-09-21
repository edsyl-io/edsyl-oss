using System.Diagnostics.CodeAnalysis;
using Cortex.Net;
using Cortex.Net.Types;

namespace EdSyl.MobX;

/// <summary> Box to store primitive values. </summary>
/// <param name="current">Current value.</param>
/// <typeparam name="T">Type of the property.</typeparam>
/// <remarks>https://mobx.js.org/api.html#observablebox</remarks>
public sealed class Box<T>(T? current = default) : Atom, IObservableValue<T>
{
    private T? current = current;

    [SuppressMessage("Usage", "CA2225", Justification = "Syntactic Sugar")]
    public static implicit operator T(Box<T> box) => box.Value;

    public event EventHandler<ValueChangeEventArgs<T>>? Change
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    public event EventHandler<ValueChangedEventArgs<T>>? Changed
    {
        add => throw new NotSupportedException();
        remove => throw new NotSupportedException();
    }

    /// <inheritdoc />
    public T Value
    {
        get => Get();
        set => Set(value);
    }

    /// <inheritdoc />
    object IValue.Value
    {
        get => Get()!;
        set => Set((T)value);
    }

    /// <inheritdoc />
    public void Observe(EventHandler<ValueChangedEventArgs<T>> changedEventHandler, bool fireImmediately)
    {
        throw new NotSupportedException();
    }

    /// <summary> Get property value while tracking a read access. </summary>
    private T Get()
    {
        ReportObserved();
        return current!;
    }

    /// <summary> Set the property value to a new one while reporting changes if any. </summary>
    /// <param name="value">Value to set.</param>
    private void Set(T value)
    {
        (var last, current) = (current, value);
        if (EqualityComparer<T>.Default.Equals(last, value)) return;
        CheckIfStateModificationsAreAllowed();
        ReportChanged();
    }
}
