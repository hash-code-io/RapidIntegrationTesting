using Microsoft.Extensions.DependencyInjection;
using Testing.Integration.TestWebApi.Data;

namespace RapidIntegrationTesting.Integration.Tests.Tests.Seeder;

internal static class DbExecutor
{
    public static async Task ExecuteIsolated(this TestWebAppFactory webAppFactory, Func<TestDbContext, Task> action)
    {
        using IServiceScope scope = webAppFactory.Services.CreateScope();
        TestDbContext dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        await action(dbContext);
    }
}