using System.Collections;

namespace EdSyl;

public static class Lists
{
    public static void Insert<T>(this IList<T> list, Index index, T item)
    {
        list.Insert(index.GetOffset(list.Count), item);
    }

    public static int IndexOf<T>(this IList<T> list, Predicate<T> predicate)
    {
        for (var i = 0; i < list.Count; i++)
            if (predicate(list[i]))
                return i;

        return -1;
    }

    [SuppressMessage("Design", "CA1002", Justification = "Performance")]
    [SuppressMessage("Design", "MA0016", Justification = "Performance")]
    public static List<T> Tap<T>(this List<T> list, Action<T> action)
    {
        list.ForEach(action);
        return list;
    }

    public static T? Nullify<T>(this T list) where T : ICollection
        => list.Count > 0 ? list : default;

    public static int RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
    {
        if (list is List<T> basic)
            return basic.RemoveAll(predicate);

        var count = 0;
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (!predicate(list[i])) continue;
            list.RemoveAt(i);
            count++;
        }

        return count;
    }

    public static void RemoveAllAfter<T>(this LinkedList<T> list, T item)
    {
        var current = list.Find(item);
        while (current?.Next != null)
            list.Remove(current.Next);
    }
}
