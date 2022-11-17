using System.Reflection;

namespace RapidIntegrationTesting.Utility.Seeder;

/// <summary>
///     Function to retrieve Ids for given entity
/// </summary>
/// <typeparam name="TEntity">The type of entity</typeparam>
public delegate object[] EntityIdRetriever<in TEntity>(TEntity entity) where TEntity : class;

internal static class EntityIdRetrievers
{
    public static EntityIdRetriever<TEntity> Reflection<TEntity>(string[] idPropertyNames) where TEntity : class
    {
        ValidateProperties<TEntity>(idPropertyNames);
        return entity =>
        {
            object[] ids = new object[idPropertyNames.Length];
            for (int index = 0; index < idPropertyNames.Length; index++)
            {
                object? id = typeof(TEntity).GetProperty(idPropertyNames[index])?.GetValue(entity);
                ids[index] = id ?? throw new InvalidOperationException("Entity property unexpectedly returned null");
            }

            return ids;
        };
    }

    private static void ValidateProperties<TEntity>(IEnumerable<string> idPropertyNames)
    {
        foreach (string idPropertyName in idPropertyNames)
        {
            PropertyInfo? prop = typeof(TEntity).GetProperty(idPropertyName);
            if (prop is null) throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} did not include a property with name {idPropertyName}");
        }
    }
}