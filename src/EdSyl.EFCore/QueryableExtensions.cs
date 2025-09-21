using Microsoft.EntityFrameworkCore.Query;

namespace EdSyl.EFCore;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> Include<TEntity>(
        this IQueryable<TEntity> source,
        [NotParameterized] IEnumerable<string?>? properties)
        where TEntity : class
    {
        if (properties != null)
            foreach (var property in properties)
                if (property != null)
                    source = source.Include(property);

        return source;
    }
}
