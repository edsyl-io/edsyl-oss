using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EdSyl.EFCore;

[SuppressMessage("Trimming", "IL2091", Justification = "Validated by " + nameof(DbContext))]
public static class DbExtensions
{
    public static void TryRemoveRange(this DbContext db, IEnumerable<object>? entities)
    {
        if (entities != null)
            db.RemoveRange(entities);
    }

    public static void InsertRange<T>(this DbContext db, IEnumerable<T> entities, Action<EntityEntry<T>> process) where T : class
    {
        foreach (var entity in entities)
            process(db.Add(entity));
    }

    public static async Task RemoveMany<T>(this DbContext db, IQueryable<T> query, CancellationToken cancellation = default)
    {
        db.RemoveRange(await query.ToListAsync(cancellation));
    }

    public static async Task<T> GetAsync<T>(this DbSet<T> source, object key, CancellationToken cancellationToken = default) where T : class
    {
        return await source.FindAsync(new[] { key }, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(T));
    }

    public static async Task<T> GetAsync<T>(this DbSet<T> source, object[] key, CancellationToken cancellationToken = default) where T : class
    {
        return await source.FindAsync(key, cancellationToken)
            ?? throw new EntityNotFoundException(typeof(T));
    }

    public static async Task<T> FirstEntity<T>(this IQueryable<T> source, CancellationToken cancellation = default)
    {
        return await source.FirstOrDefaultAsync(cancellation)
            ?? throw new EntityNotFoundException(typeof(T));
    }

    public static async Task<T> FirstEntity<T>(this IAsyncEnumerable<T> source, CancellationToken cancellation = default)
    {
        return await source.FirstOrDefaultAsync(cancellation)
            ?? throw new EntityNotFoundException(typeof(T));
    }
}
