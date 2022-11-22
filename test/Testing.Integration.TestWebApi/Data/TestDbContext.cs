using Microsoft.EntityFrameworkCore;

namespace Testing.Integration.TestWebApi.Data;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<UserData> UserDatas => Set<UserData>();
    public DbSet<AdminData> AdminDatas => Set<AdminData>();
    public DbSet<ComplicatedData> ComplicatedDatas => Set<ComplicatedData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

        modelBuilder.Entity<UserData>().HasData(UserData.SeedData);
        modelBuilder.Entity<AdminData>().HasData(AdminData.SeedData);

        modelBuilder.Entity<ComplicatedData>(b =>
        {
            b.OwnsOne(x => x.Name, iob =>
            {
                iob.HasData(ComplicatedData.OwnedEntitySeedData);
            });
            b.HasData(ComplicatedData.SeedData);
        });
    }
}