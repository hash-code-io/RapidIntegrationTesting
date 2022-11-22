namespace Testing.Integration.TestWebApi.Data;

public class UserData
{
    public Guid Id { get; set; }
    public string SomeData { get; set; } = "";

    public static UserData[] SeedData => new[]
    {
        new UserData { Id = Guid.Parse("bc09de77-2327-45ff-bc7b-57f43841646b"), SomeData = "SomeData1" },
        new UserData { Id = Guid.Parse("8b125f8a-83c5-4917-988d-2ecdba04ef3a"), SomeData = "SomeData2" },
        new UserData { Id = Guid.Parse("33cc0259-2641-43a1-8191-78343c28d3d1"), SomeData = "SomeData3" }
    };
}