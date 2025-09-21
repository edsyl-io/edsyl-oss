using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.EFCore;

public sealed class DbContextProvider : IDbContextProvider
{
    private static readonly Func<DbContext, Task> DestroyAsync = x => x.DisposeAsync().AsTask();

    private bool closed;
    private IServiceProvider provider;
    private HashSet<DbContext>? contexts;

    public DbContextProvider(IServiceProvider provider)
    {
        this.provider = provider;
    }

    /// <inheritdoc />
    public TContext CreateDbContext<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors | PublicProperties)] TContext>() where TContext : DbContext
    {
        if (closed) throw new ObjectDisposedException(GetType().Name);
        var factory = provider.GetRequiredService<IDbContextFactory<TContext>>();
        var context = factory.CreateDbContext();
        (contexts ??= new()).Add(context);
        return context;
    }

    /// <inheritdoc />
    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        if (closed) throw new ObjectDisposedException(GetType().Name);
        if (contexts is not { Count: > 0 }) return 0;
        var tasks = contexts.Select(context => context.SaveChangesAsync(cancellationToken));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.Sum();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(this, contexts);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return DisposeAsync(this, contexts);
    }

    private void Close()
    {
        closed = true;
        provider = null!;
        contexts = null;
    }

    private static void Dispose(DbContextProvider instance, HashSet<DbContext>? contexts)
    {
        instance.Close();

        if (contexts is { Count: > 0 })
            foreach (var context in contexts)
                context.Dispose();
    }

    private static ValueTask DisposeAsync(DbContextProvider instance, HashSet<DbContext>? contexts)
    {
        instance.Close();
        return contexts is { Count: > 0 }
            ? new(Task.WhenAll(contexts.Select(DestroyAsync)))
            : default;
    }

    [SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
    [SuppressMessage("Usage", "CA1812", Justification = "Instantiated by DI container.")]
    internal class Pooled
        <[DynamicallyAccessedMembers(NonPublicConstructors | None | PublicConstructors | PublicParameterlessConstructor | PublicProperties)] TContext>
        : IDbContextFactory<TContext>, IDbContextPool<TContext> where TContext : DbContext
    {
        private readonly DbContextProvider provider;
        private readonly IDbContextPool<TContext> pool;
        private readonly PooledDbContextFactory<TContext> factory;

        public Pooled(IDbContextProvider provider, IDbContextPool<TContext> pool)
        {
            this.pool = pool;
            this.provider = (DbContextProvider)provider;
            factory = new(this);
        }

        public TContext CreateDbContext()
        {
            var context = factory.CreateDbContext();
            provider.contexts?.Add(context);
            return context;
        }

        public IDbContextPoolable Rent()
        {
            return pool.Rent();
        }

        public void Return(IDbContextPoolable context)
        {
            if (context is DbContext dbContext)
                provider.contexts?.Remove(dbContext);

            pool.Return(context);
        }

        public ValueTask ReturnAsync(IDbContextPoolable context, CancellationToken cancellationToken = default)
        {
            if (context is DbContext dbContext)
                provider.contexts?.Remove(dbContext);

            return pool.ReturnAsync(context, cancellationToken);
        }
    }
}
