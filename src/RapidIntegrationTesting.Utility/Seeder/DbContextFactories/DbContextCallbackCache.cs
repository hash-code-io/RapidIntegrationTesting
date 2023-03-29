using Microsoft.EntityFrameworkCore;

namespace RapidIntegrationTesting.Utility.Seeder.DbContextFactories;

/// <summary>
///     Callback to create a DbContext
/// </summary>
/// <typeparam name="TDbContext">The type of DbContext</typeparam>
/// <returns>The create DbContext</returns>
public delegate TDbContext DbContextCreator<out TDbContext>() where TDbContext : DbContext;

internal sealed class DbContextCallbackCache<TDbContext> : IDbContextCache<TDbContext> where TDbContext : DbContext
{
    private readonly DbContextCreator<TDbContext> _creator;
    private TDbContext? _context;

    public DbContextCallbackCache(DbContextCreator<TDbContext> creator) => _creator = creator ?? throw new ArgumentNullException(nameof(creator));

    public void Dispose() => _context?.Dispose();

    public TDbContext GetCachedDbContext() => _context ??= _creator();

    public void ClearCache() => _context = null;
}