using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.EFCore;

public static class DbServiceCollectionExtensions
{
    public static void AddPooledDbContext<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors | PublicProperties)] TContext>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder>? options = null)
        where TContext : DbContext
    {
        services.AddPooledDbContext<TContext, TContext>(options);
    }

    [SuppressMessage("Usage", "IL2091", Justification = "Verified")]
    [SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
    public static void AddPooledDbContext<TService, [DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors | PublicProperties)] TContext>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder>? options = null)
        where TService : DbContext
        where TContext : TService
    {
        options ??= (_, _) => { };
        services.AddDbContextPool<TService, TContext>(options);
        services.AddDbContextFactory<TService, DbFactoryProxy<TService, TContext>>();
        services.AddPooledDbContextFactory<TContext>(options);
    }

    [SuppressMessage("Usage", "IL2091", Justification = "Verified")]
    private sealed class DbFactoryProxy<TService, TContext>(IDbContextFactory<TContext> service) : IDbContextFactory<TService>
        where TService : DbContext
        where TContext : TService
    {
        /// <inheritdoc />
        public TService CreateDbContext() => service.CreateDbContext();
    }
}
