using RapidIntegrationTesting.Utility.Seeder;
using Testing.Integration.TestWebApi.Data;

namespace RapidIntegrationTesting.Integration.Tests.Tests.Seeder;

internal class TestSeederImplementation : TestSeeder<TestDbContext>
{
    public TestSeederImplementation(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public TestSeederImplementation CreateComplicatedData(Guid id)
    {
        Create(new ComplicatedData { Id = id, Name = new Name("Herp", "Derp")});
        return this;
    }
}