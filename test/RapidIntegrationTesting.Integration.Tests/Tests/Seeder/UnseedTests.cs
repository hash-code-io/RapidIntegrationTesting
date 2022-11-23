using Microsoft.EntityFrameworkCore;
using Testing.Integration.TestWebApi.Data;

namespace RapidIntegrationTesting.Integration.Tests.Tests.Seeder;

public class UnseedTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly Guid _createdId = Guid.Parse("99b71010-c86e-4082-90b7-86eeaef9a41c");
    private readonly TestWebAppFactory _factory;

    public UnseedTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task Should_Unseed()
    {
        // Arrange
        var seeder = new TestSeederImplementation(_factory.Services);
        seeder.Unseed<UserData>(() => new object[] { _createdId });
        await seeder.Execute();

        await _factory.ExecuteIsolated(async db =>
        {
            var userData = new UserData { Id = _createdId, SomeData = "Test" };
            db.UserDatas.Add(userData);
            await db.SaveChangesAsync();
        });

        // Act
        await seeder.DisposeAsync();

        // Assert
        await _factory.ExecuteIsolated(async db =>
        {
            bool found = await db.UserDatas.AnyAsync(x => x.Id == _createdId);
            Assert.False(found);
        });
    }
}