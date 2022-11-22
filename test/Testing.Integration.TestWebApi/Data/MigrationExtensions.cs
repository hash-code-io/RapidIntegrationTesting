using Microsoft.EntityFrameworkCore;

namespace Testing.Integration.TestWebApi.Data;

public static class MigrationExtensions
{
    public static IHost MigrateHostedDbContext<TContext>(this IHost host) where TContext : DbContext
    {
        if (host == null)
            throw new ArgumentNullException(nameof(host));
        using IServiceScope scope = host.Services.CreateScope();
        TestDbContext dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return host;
    }
}
