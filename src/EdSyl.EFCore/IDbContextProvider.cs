using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.EFCore;

/// <summary>
/// Encapsulates control over all <see cref="DbContext" /> instances available within a scope.
/// </summary>
public interface IDbContextProvider : IDisposable, IAsyncDisposable
{
    /// <inheritdoc cref="IDbContextFactory{TContext}.CreateDbContext" />
    TContext CreateDbContext<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors | PublicProperties)] TContext>() where TContext : DbContext;

    /// <summary> Saves all changes made to ALL contexts to the database within the current scope. </summary>
    /// <param name="cancellationToken">A cancellation to observe while waiting for the task to complete.</param>
    /// <returns>The number of entries written to the database.</returns>
    /// <seealso cref="DbContext.SaveChangesAsync(CancellationToken)" />
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}
