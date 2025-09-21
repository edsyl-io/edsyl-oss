namespace EdSyl.Linq;

public static class QueryableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> Never<T>(this IQueryable<T> query)
    {
        return query.Where(Cache<T>.Never);
    }
}

file static class Cache<T>
{
    public static readonly Expression<Func<T, bool>> Never = Expression.Lambda<Func<T, bool>>(
        Expression.Constant(false),
        Expression.Parameter(typeof(T), default)
    );
}
