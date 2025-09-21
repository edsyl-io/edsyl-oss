using EdSyl.Linq;
using EdSyl.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EdSyl.EFCore.Services;

[SuppressMessage("Trimming", "IL2087", Justification = "Defined by " + nameof(DbContext))]
[SuppressMessage("Trimming", "IL2091", Justification = "Defined by " + nameof(DbContext))]
public abstract class EfService<T>(DbContext db) where T : class
{
    /// <summary>
    /// Database context containing the set of <see cref="T" /> entities.
    /// </summary>
    protected DbContext Db { get; } = db;

    /// <summary>
    /// A set that can be used to query and save instances of <see cref="T" />.
    /// </summary>
    protected DbSet<T> Set { get; } = db.Set<T>();

    /// <summary> Create a search query with default settings. </summary>
    public virtual IQueryable<T> Search() => Set;

    /// <summary> Inserts a new entity into a database. </summary>
    /// <param name="entity">An entity to insert.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An inserted entity.</returns>
    public async Task<T> InsertAsync(T entity, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        entity = Db.Add(entity).Entity;
        await Db.SaveChangesAsync(cancellation);
        return entity;
    }

    /// <summary> Inserts multiple entities into a database in a batch. </summary>
    /// <param name="entities">A collection of entities to insert.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of inserted entities.</returns>
    public async Task<ICollection<T>> InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        var collection = entities.AsCollection();
        Db.AddRange(collection);
        await Db.SaveChangesAsync(cancellation);
        return collection;
    }

    /// <summary> Inserts multiple entities into a database in a batch. </summary>
    /// <param name="entities">A collection of entities to insert.</param>
    /// <param name="process">Action to run for every entry before save.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of inserted entities.</returns>
    public async Task<ICollection<T>> InsertRangeAsync(IEnumerable<T> entities, Action<EntityEntry<T>> process, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        var collection = entities.AsCollection();
        Db.InsertRange(collection, process);
        await Db.SaveChangesAsync(cancellation);
        return collection;
    }

    /// <summary> Update an existing entity in a database. </summary>
    /// <param name="entity">An entity to update.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An updated entity.</returns>
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        entity = Db.Update(entity).Entity;
        await Db.SaveChangesAsync(cancellation);
        return entity;
    }

    /// <summary> Update multiple entities in a database in a batch. </summary>
    /// <param name="entities">A collection of entities to update.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of updated entities.</returns>
    public async Task<ICollection<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        var collection = entities.AsCollection();
        Db.UpdateRange(collection);
        await Db.SaveChangesAsync(cancellation);
        return collection;
    }

    /// <summary> Delete an existing entity from a database. </summary>
    /// <param name="entity">An entity to delete.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A deleted entity.</returns>
    public async Task<T> DeleteAsync(T entity, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        entity = Db.Remove(entity).Entity;
        await Db.SaveChangesAsync(cancellation);
        return entity;
    }

    /// <summary> Delete multiple entities from a database in a batch. </summary>
    /// <param name="entities">A collection of entities to remove.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of deleted entities.</returns>
    public async Task<ICollection<T>> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        var collection = entities.AsCollection();
        Set.RemoveRange(collection);
        await Db.SaveChangesAsync(cancellation);
        return collection;
    }
}

/// <summary> Service to perform CRUD operations on <see cref="T" /> entities. </summary>
/// <typeparam name="T">Type of the entity in the database.</typeparam>
/// <typeparam name="TKey">Primary key of the entity.</typeparam>
/// <param name="db">Database context containing the set of <see cref="T" /> entities.</param>
[SuppressMessage("Trimming", "IL2087", Justification = "Defined by " + nameof(DbContext))]
[SuppressMessage("Trimming", "IL2091", Justification = "Defined by " + nameof(DbContext))]
public abstract class EfService<T, TKey>(DbContext db) : EfService<T>(db)
    where T : class
    where TKey : struct
{
    private string? keyName;
    private Func<T, TKey>? keyOf;

    /// <summary> Name of the entity primary key. </summary>
    private string KeyName => keyName ??= Db.Model
        .FindEntityType(typeof(T))!
        .FindPrimaryKey()!
        .Properties.Single().Name;

    /// <summary> Function to access the primary key of the entity. </summary>
    private Func<T, TKey> KeyOf => keyOf ??= typeof(T).RequireAccessor(KeyName).Getter<T, TKey>();

    /// <summary> Create a query to select entities having the specified primary key. </summary>
    /// <param name="key">A primary key to be found.</param>
    public IQueryable<T> SearchKey(TKey key)
        => Search().Where(e => EF.Property<TKey>(e, KeyName).Equals(key));

    /// <summary> Create a query to select entities having any of the provided keys. </summary>
    /// <param name="keys">Collection of primary keys to look for.</param>
    public IQueryable<T> SearchKeys(ICollection<TKey> keys)
        => Search().Where(e => keys.Contains(EF.Property<TKey>(e, KeyName)));

    /// <summary>
    /// <inheritdoc cref="DbSet{T}.FindAsync(object[], CancellationToken)" />
    /// </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An entity found by the primary key on success; null otherwise.</returns>
    public ValueTask<T?> FindAsync(TKey key, CancellationToken cancellation = default)
    {
        return Set.FindAsync([key], cancellation);
    }

    /// <summary>
    /// <inheritdoc cref="DbSet{T}.FindAsync(object[], CancellationToken)" />
    /// </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An entity found by the primary key.</returns>
    /// <exception cref="EntityNotFoundException">When the entity by the given primary key doesn't exist.</exception>
    public async Task<T> GetAsync(TKey key, CancellationToken cancellation = default)
    {
        return await FindAsync(key, cancellation)
            ?? throw new EntityNotFoundException(typeof(T));
    }

    /// <summary> Get entities by respective primary keys preserving the order of the keys. </summary>
    /// <param name="keys">Collection of keys to look for.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <exception cref="EntityNotFoundException">When any of the entities haven't been found.</exception>
    public async Task<ICollection<T>> GetAsync(ICollection<TKey> keys, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();

        var entities = await SearchKeys(keys).ToDictionaryAsync(KeyOf, cancellation);
        if (entities.Count != keys.Count)
            throw new EntityNotFoundException(typeof(T));

        var results = new List<T>(keys.Count);
        foreach (var key in keys)
        {
            if (!entities.TryGetValue(key, out var entity))
                throw new EntityNotFoundException(typeof(T));

            results.Add(entity);
        }

        return results;
    }

    /// <summary> Update an entity by the given primary key. </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="mutation">An action to perform on a found entity before saving to the database.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An updated entity.</returns>
    /// <exception cref="EntityNotFoundException">When an entity by the given primary key doesn't exist.</exception>
    public async Task<T> UpdateAsync(TKey key, Action<T> mutation, CancellationToken cancellation = default)
    {
        var entity = await GetAsync(key, cancellation);
        cancellation.ThrowIfCancellationRequested();
        mutation(entity);
        return await UpdateAsync(entity, cancellation);
    }

    /// <summary> Update an entity by the given primary key. </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="props">Properties to apply to an entity by the primary key.</param>
    /// <param name="mutation">An action to perform on a found entity before saving to the database.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>An updated entity.</returns>
    /// <exception cref="EntityNotFoundException">When an entity by the given primary key doesn't exist.</exception>
    /// <typeparam name="TProps">A data container with properties to apply for an entity.</typeparam>
    public async Task<T> UpdateAsync<TProps>(TKey key, TProps props, Action<TProps, T> mutation, CancellationToken cancellation = default)
    {
        var entity = await GetAsync(key, cancellation);
        cancellation.ThrowIfCancellationRequested();
        mutation(props, entity);
        return await UpdateAsync(entity, cancellation);
    }

    /// <summary> Update multiple entities in a database by respective properties. </summary>
    /// <param name="data">A collection of properties to apply by a respective primary key.</param>
    /// <param name="mutation">An action to perform on every entity by a respective primary key.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of updated entities.</returns>
    /// <exception cref="EntityNotFoundException">When any of the entities haven't been found.</exception>
    /// <typeparam name="TProps">A data container with properties to apply for an entity.</typeparam>
    public async Task<ICollection<T>> UpdateRangeAsync<TProps>(IDictionary<TKey, TProps> data, Action<TProps, T> mutation, CancellationToken cancellation = default)
    {
        var results = await GetAsync(data.Keys, cancellation);
        cancellation.ThrowIfCancellationRequested();

        var key = KeyOf;
        foreach (var entity in results)
            mutation(data[key(entity)], entity);

        return await UpdateRangeAsync(results, cancellation);
    }

    /// <summary> Delete an entity by the given primary key. </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A deleted entity.</returns>
    /// <exception cref="EntityNotFoundException">When the entity by the given primary key doesn't exist.</exception>
    public async Task<T> DeleteAsync(TKey key, CancellationToken cancellation = default)
    {
        var entity = await GetAsync(key, cancellation);
        return await DeleteAsync(entity, cancellation);
    }

    /// <summary> Try to delete an entity by the primary key if such exists. </summary>
    /// <param name="key">A primary key to be found.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A deleted entity on success; null otherwise.</returns>
    public async Task<T?> TryDeleteAsync(TKey key, CancellationToken cancellation = default)
    {
        var entity = await FindAsync(key, cancellation);
        return entity != null
            ? await DeleteAsync(entity, cancellation)
            : null;
    }

    /// <summary>  Delete multiple entities from a database in a batch. </summary>
    /// <param name="keys">A collection of primary keys to delete.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of deleted entities.</returns>
    /// <exception cref="EntityNotFoundException">When any of the entities haven't been found.</exception>
    public async Task<ICollection<T>> DeleteRangeAsync(ICollection<TKey> keys, CancellationToken cancellation = default)
    {
        var entities = await GetAsync(keys, cancellation);
        return await DeleteRangeAsync(entities, cancellation);
    }
}
