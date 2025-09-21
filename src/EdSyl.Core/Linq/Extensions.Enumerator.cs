namespace EdSyl.Linq;

public static partial class Extensions
{
    /// <summary> Seek to first element matching the given predicate. </summary>
    /// <param name="iterator">Iterator to traverse..</param>
    /// <param name="predicate">A delegate defining a criteria to match.</param>
    /// <typeparam name="T">Type of the iterable elements.</typeparam>
    /// <returns>True when any element of the iterator matched the predicate; false otherwise. </returns>
    public static bool Seek<T>(this IEnumerator<T> iterator, Func<T, bool>? predicate)
    {
        if (predicate == null)
            return true;

        do
        {
            if (predicate(iterator.Current))
                return true;
        } while (iterator.MoveNext());

        return false;
    }
}
