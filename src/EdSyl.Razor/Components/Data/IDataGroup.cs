namespace EdSyl.Razor.Components;

public interface IDataGroup<in T>
{
    /// <summary> Notify that item content has been changed. </summary>
    /// <param name="item">A modified item.</param>
    void NotifyItemChanged(T item);
}
