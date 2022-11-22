using Microsoft.EntityFrameworkCore;

namespace RapidIntegrationTesting.Integration.Tests.Tests.Seeder;

[Collection(WebAppFactoryCollectionFixture.Name)]
public class CreateDataTest
{
    private readonly HttpClient _client;
    private readonly Guid _createdId = Guid.Parse("22b71010-c86e-4082-90b7-86eeaef9a41c");
    private readonly TestWebAppFactory _factory;

    public CreateDataTest(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_And_Unseed_Data()
    {
        // Arrange
        var seeder = new TestSeederImplementation(_factory.Services);
        seeder.CreateComplicatedData(_createdId);
        await seeder.Execute();

        // Act && Assert
        await _factory.ExecuteIsolated(async db =>
        {
            bool found = await db.ComplicatedDatas.AnyAsync(x => x.Id == _createdId);
            Assert.True(found);
        });

        // Act
        await seeder.DisposeAsync();

        // Assert
        await _factory.ExecuteIsolated(async db =>
        {
            bool found = await db.ComplicatedDatas.AnyAsync(x => x.Id == _createdId);
            Assert.False(found);
        });
    }
}