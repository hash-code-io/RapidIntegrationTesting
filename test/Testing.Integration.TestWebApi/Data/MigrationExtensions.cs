using Microsoft.EntityFrameworkCore;

namespace Testing.Integration.TestWebApi.Data;

public static class MigrationExtensions
{
    private static bool _migrated;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public static async Task MigrateHostedDbContext<TContext>(this IHost host) where TContext : DbContext
    {
        if (host == null)
            throw new ArgumentNullException(nameof(host));

        try
        {
            await Semaphore.WaitAsync();
            if (_migrated) return;

            using IServiceScope scope = host.Services.CreateScope();
            TestDbContext dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            _migrated = true;
        }
        finally
        {
            Semaphore.Release();
        }
    }
}