using Microsoft.EntityFrameworkCore;

namespace RapidIntegrationTesting.Utility.Seeder.DbContextFactories;

internal interface IDbContextCache<out TDbContext> : IDisposable
    where TDbContext : DbContext
{
    TDbContext GetCachedDbContext();
    void ClearCache();
}