using EnsekExercise.DbEntities;
using EnsekExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsApi.Tests.DbEntities;

public class EnsekDbEntityTests
{
    [Fact]
    public void OnModelCreating_SeedData_Success()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<EnsekDbEntity>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        var mockAccounts = new List<Account>
        {
            new()
            {
                AccountId = 2344,
                FirstName = "Tommy",
                LastName = "Test"
            },
            new()
            {
                AccountId = 2233,
                FirstName = "Barry",
                LastName = "Test"
            },
            new()
            {
                AccountId = 8766,
                FirstName = "Sally",
                LastName = "Test"
            }
        };

        var mockMeterReadings = new List<MeterReading>();

        // Act: Create the in-memory database context and add test seed data
        using var dbContext = new EnsekDbEntity(options);
        dbContext.ManageEntities(new ModelBuilder());
        dbContext.SeedData("../../../TestData/Accounts.csv");
        dbContext.SaveChanges();

        // Assert
        Assert.NotEmpty(dbContext.Accounts);
        Assert.Empty(dbContext.MeterReadings);
        Assert.Equivalent(dbContext.Accounts, mockAccounts);
        Assert.Equivalent(dbContext.MeterReadings, mockMeterReadings);
    }
}