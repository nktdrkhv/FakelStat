using FakelStat.Repositories;
using FakelStat.Helpers;
using PetaPoco;
using PetaPoco.Providers;

namespace FakelStat.Tests;

public class SqlMomentumLoadRepositoryTests
{
    private readonly IDatabase _database;

    public SqlMomentumLoadRepositoryTests()
    {
        _database = DatabaseConfiguration.Build()
         .UsingConnectionString("Data Source=Files/fakel.sqlite3;Version=3;")
         .UsingProvider<SQLiteDatabaseProvider>()
         .UsingDefaultMapper<ConventionMapper>(PetaPocoHelpers.ConfigureMapper)
         .WithoutAutoSelect()
         .Create();
    }

    [Fact]
    public async void CanGetMomentumLoad()
    {
        var repo = new SqlMomentumLoadRepository(_database);
        var id = 50;
        var momentumLoad = await repo.GetByIdAsync(id);
        Assert.Equal(20, momentumLoad.Amount);
    }

    [Fact]
    public async void CanFilterDays()
    {
        var repo = new SqlMomentumLoadRepository(_database);
        var days = await repo.GetDaysAsync(
            DayOfWeek.Monday,
            DateOnly.FromDateTime(new DateTime(2024, 2, 7)),
            DateOnly.FromDateTime(new DateTime(2024, 2, 21)));
        Assert.Equal(192, days.Count());
    }
}