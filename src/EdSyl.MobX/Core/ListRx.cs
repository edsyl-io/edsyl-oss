using System.Collections;
using Cortex.Net;

namespace EdSyl.MobX;

public sealed class ListRx<T> : Atom, IList<T>, IReadOnlyList<T>, IList, IReactiveObject
{
    private readonly IList<T> list;

    public ListRx() => list = [];
    public ListRx(IList<T> list) => this.list = list;

    /// <inheritdoc cref="IList.IsReadOnly" />
    public bool IsReadOnly => list.IsReadOnly;

    /// <inheritdoc cref="IList.IsSynchronized" />
    public bool IsSynchronized => false;

    /// <inheritdoc cref="IList.SyncRoot" />
    public object SyncRoot { get; } = new();

    /// <inheritdoc cref="IList.IsFixedSize" />
    public bool IsFixedSize => false;

    /// <inheritdoc cref="ICollection.Count" />
    public int Count => Access(list.Count);

    /// <inheritdoc cref="IList{T}.this" />
    public T this[int index]
    {
        get => Access(list[index]);
        set
        {
            list[index] = value;
            ReportChanged();
        }
    }

    /// <inheritdoc />
    object? IList.this[int index]
    {
        get => Access(((IList)list)[index]);
        set
        {
            ((IList)list)[index] = value;
            ReportChanged();
        }
    }

    /// <inheritdoc />
    public bool Contains(T item)
        => Access(list.Contains(item));

    /// <inheritdoc />
    bool IList.Contains(object? value)
        => Access(((IList)list).Contains(value));

    /// <inheritdoc />
    public int IndexOf(T item)
        => Access(list.IndexOf(item));

    /// <inheritdoc />
    int IList.IndexOf(object? value)
        => Access(((IList)list).IndexOf(value));

    /// <inheritdoc />
    public void Add(T item)
    {
        list.Add(item);
        ReportChanged();
    }

    /// <inheritdoc />
    int IList.Add(object? value)
    {
        var result = ((IList)list).Add(value);
        ReportChanged();
        return result;
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        list.Insert(index, item);
        ReportChanged();
    }

    /// <inheritdoc />
    void IList.Insert(int index, object? value)
    {
        ((IList)list).Insert(index, value);
        ReportChanged();
    }

    /// <inheritdoc />
    void IList.RemoveAt(int index)
    {
        ((IList)list).RemoveAt(index);
        ReportChanged();
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        if (!list.Remove(item)) return false;
        ReportChanged();
        return true;
    }

    /// <inheritdoc />
    void IList.Remove(object? value)
    {
        var count = list.Count;
        ((IList)list).Remove(value);
        if (count != list.Count)
            ReportChanged();
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        ReportChanged();
    }

    /// <inheritdoc cref="IList.Clear" />
    public void Clear()
    {
        var count = list.Count;
        list.Clear();
        if (list.Count != count)
            ReportChanged();
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
        => list.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    void ICollection.CopyTo(Array array, int index)
        => ((ICollection)list).CopyTo(array, index);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        ReportObserved();
        return list.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        ReportObserved();
        return list.GetEnumerator();
    }
}
