namespace RapidIntegrationTesting.Integration.Tests;

[CollectionDefinition(Name)]
public class WebAppFactoryCollectionFixture : ICollectionFixture<TestWebAppFactory>
{
    public const string Name = "WebAppFactoryCollection";
}