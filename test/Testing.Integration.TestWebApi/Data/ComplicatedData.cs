namespace Testing.Integration.TestWebApi.Data;

public record Name(string FirstName, string LastName);

public class ComplicatedData
{
    public Guid Id { get; set; }
    public Name Name { get; set; } = null!;

    private static ComplicatedData[] _data => new[]
    {
        new ComplicatedData { Id = Guid.Parse("dcdaef51-0cff-46e1-9892-41cd6bb9d768"), Name = new Name("Bobby", "Bob") },
        new ComplicatedData { Id = Guid.Parse("d71b4d8a-e0c4-44ba-9600-e3ef31b5bcf1"), Name = new Name("Alice", "Wonderland") }
    };

    public static object[] OwnedEntitySeedData => _data.Select(x => new { x.Name.FirstName, x.Name.LastName, ComplicatedDataId = x.Id } as object).ToArray();
    public static object[] SeedData => _data.Select(x => new { x.Id } as object).ToArray();
}