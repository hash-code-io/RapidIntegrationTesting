namespace Testing.Integration.TestWebApi.Data;

public class AdminData
{
    public Guid Id { get; set; }
    public string SomeData { get; set; } = "";

    public static AdminData[] SeedData => new[]
    {
        new AdminData { Id = Guid.Parse("2c09de77-2327-45ff-bc7b-57f43841646b"), SomeData = "SomeData1" },
        new AdminData { Id = Guid.Parse("3b125f8a-83c5-4917-988d-2ecdba04ef3a"), SomeData = "SomeData2" },
        new AdminData { Id = Guid.Parse("43cc0259-2641-43a1-8191-78343c28d3d1"), SomeData = "SomeData3" }
    };
}