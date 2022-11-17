using Microsoft.EntityFrameworkCore;

namespace RapidIntegrationTesting.Utility.Seeder;

internal record UnseedInfo(Type EntityType, IdRetriever IdRetriever);

internal class Unseeder
{
    private readonly List<UnseedInfo> _unseedInfos = new();

    public async Task Unseed(DbContext context)
    {
        _unseedInfos.Reverse();
        foreach ((Type entityType, IdRetriever idRetriever) in _unseedInfos)
        {
            object[] id = idRetriever();
            object? entity = await context.FindAsync(entityType, id).ConfigureAwait(false);
            if (entity is not null) context.Remove(entity);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        _unseedInfos.Clear();
    }

    public void Add(UnseedInfo unseedInfo) => _unseedInfos.Add(unseedInfo);
}