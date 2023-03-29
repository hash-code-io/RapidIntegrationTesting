using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RapidIntegrationTesting.Utility.Seeder.DbContextFactories;

internal sealed class DbContextServiceProviderCache<TDbContext> : IDbContextCache<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private IServiceScope _scope;

    public DbContextServiceProviderCache(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _scope = serviceProvider.CreateScope();
    }

    public void Dispose() => _scope.Dispose();
    public TDbContext GetCachedDbContext() => _scope.ServiceProvider.GetRequiredService<TDbContext>();

    public void ClearCache()
    {
        _scope.Dispose();
        _scope = _serviceProvider.CreateScope();
    }
}