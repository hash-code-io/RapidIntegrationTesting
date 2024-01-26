using Microsoft.EntityFrameworkCore;
using RapidIntegrationTesting.Utility.Seeder.DbContextFactories;

namespace RapidIntegrationTesting.Utility.Seeder;

/// <summary>
///     Class used for seeding test data
/// </summary>
/// <typeparam name="TDbContext">Type of DbContext</typeparam>
public abstract class TestSeeder<TDbContext> : IAsyncDisposable
    where TDbContext : DbContext
{
    private readonly IDbContextCache<TDbContext> _dbContextCache;
    private readonly Unseeder _unseeder = new();

    /// <summary>
    ///     Constructs an instance using a service provider that has the correctly configured type of DbContext present in its services
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    protected TestSeeder(IServiceProvider serviceProvider) => _dbContextCache = new DbContextServiceProviderCache<TDbContext>(serviceProvider);

    /// <summary>
    ///     Constructs an instance using the provided creator function
    /// </summary>
    /// <param name="dbContextCreator">The creator function</param>
    protected TestSeeder(DbContextCreator<TDbContext> dbContextCreator) => _dbContextCache = new DbContextCallbackCache<TDbContext>(dbContextCreator);

    /// <summary>
    ///     The DbContext managed by this Seeder
    /// </summary>
    protected TDbContext DbContext => _dbContextCache.GetCachedDbContext();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _dbContextCache.ClearCache();
        await _unseeder.Unseed(DbContext).ConfigureAwait(false);
        _dbContextCache.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Adds an entity to "unseed". This will try to delete the entity during clean-up phase. This call is NOT required for entity added by this seeder
    /// </summary>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    /// <param name="idRetriever">A function to retrieve the entity's id</param>
    /// <returns>This instance</returns>
    public TestSeeder<TDbContext> Unseed<TEntity>(IdRetriever idRetriever)
    {
        _unseeder.Add(new UnseedInfo(typeof(TEntity), idRetriever));
        return this;
    }

    /// <summary>
    ///     Ends the configuration chain and seeds all entities
    /// </summary>
    /// <returns></returns>
    public async Task Execute() => await DbContext.SaveChangesAsync().ConfigureAwait(false);

    /// <summary>
    ///     Adds an entity to the DbContext and the internal tracker to be cleaned up during clean-up phase
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">the instance of the entity</param>
    /// <param name="idRetriever">A function to retrieve the entity's id</param>
    protected void Create<T>(T entity, EntityIdRetriever<T> idRetriever) where T : class
    {
        ArgumentNullException.ThrowIfNull(idRetriever);
        object[] id = idRetriever(entity);
        DbContext.Set<T>().Add(entity);
        _unseeder.Add(new UnseedInfo(typeof(T), () => id));
    }

    /// <summary>
    ///     Adds an entity to the DbContext and the internal tracker to be cleaned up during clean-up phase. Id will be retrieved via reflection using the given <paramref name="idPropertyName" />
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">the instance of the entity</param>
    /// <param name="idPropertyName">The name of the property holding the id</param>
    protected void Create<T>(T entity, string idPropertyName = "Id") where T : class => Create(entity, EntityIdRetrievers.Reflection<T>(new[] { idPropertyName }));

    /// <summary>
    ///     Adds an entity to the DbContext and the internal tracker to be cleaned up during clean-up phase. Id will be retrieved via reflection using the given <paramref name="idPropertyNames" />
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="entity">the instance of the entity</param>
    /// <param name="idPropertyNames">Array of keyvalues</param>
    protected void Create<T>(T entity, string[] idPropertyNames) where T : class => Create(entity, EntityIdRetrievers.Reflection<T>(idPropertyNames));

    /// <summary>
    ///     Finds a created entity via the given predicate.
    ///     <para>
    ///         NOTE: ALWAYS prefer the overload that accepts keyValues instead of this one if possible
    ///     </para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="predicate">The predicate to retrieve the entity by</param>
    /// <returns></returns>
    protected T? FindCreated<T>(Func<T, bool> predicate) where T : class => DbContext.Set<T>().FirstOrDefault(predicate);

    /// <summary>
    ///     Finds a created entity via the keyValues
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="keyValues">The keyValues to retrive the entity by</param>
    /// <returns></returns>
    protected T? FindCreated<T>(params object[] keyValues) where T : class => DbContext.Set<T>().Find(keyValues);

    /// <summary>
    ///     Finds a created entity via the given predicate, throwing an exception if the entity was not found
    ///     <para>
    ///         NOTE: ALWAYS prefer the overload that accepts keyValues instead of this one if possible
    ///     </para>
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="predicate">The predicate to retrieve the entity by</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected T FindRequiredCreated<T>(Func<T, bool> predicate) where T : class
    {
        T? entity = FindCreated(predicate);
        return entity ?? throw new InvalidOperationException("Entity could not be found");
    }

    /// <summary>
    ///     Finds a created entity via the keyValues, throwing an exception if the entity was not found
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="keyValues">The keyValues to retrive the entity by</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected T FindRequiredCreated<T>(params object[] keyValues) where T : class
    {
        T? entity = FindCreated<T>(keyValues);
        return entity ?? throw new InvalidOperationException("Entity could not be found");
    }
}