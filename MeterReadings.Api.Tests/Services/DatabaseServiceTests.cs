using EnsekExercise.DbEntities;
using EnsekExercise.Models;
using EnsekExercise.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeterReadingsApi.Tests.Services;

public class DatabaseServiceTests
{
    [Fact]
    public void GetIdRecordsWhichHaveProvidedIds_ShouldReturnMatchingIds()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DatabaseService>>();

        var options = new DbContextOptionsBuilder<EnsekDbEntity>()
            .UseInMemoryDatabase("TestDb1")
            .Options;
        
        // Act: Create the in-memory database context and add account data, which we'll use to compare
        using var dbContext = new EnsekDbEntity(options);
        dbContext.ManageEntities(new ModelBuilder());
        dbContext.SeedData("../../../TestData/Accounts.csv");

        var databaseService = new DatabaseService(dbContext, mockLogger.Object);

        // Act
        var idsToCheck = new List<int> { 2344, 2233, 8766 };
        var result = databaseService.GetIdRecordsWhichHaveProvidedIds(dbContext, idsToCheck);

        // Assert
        Assert.Equal(idsToCheck, result);
    }
    
    [Fact]
    public void GetIdRecordsWhichHaveProvidedIds_ShouldReturnSingleId()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DatabaseService>>();

        var options = new DbContextOptionsBuilder<EnsekDbEntity>()
            .UseInMemoryDatabase("TestDb2")
            .Options;
        
        // Act: Create the in-memory database context and add account data, which we'll use to compare
        using var dbContext = new EnsekDbEntity(options);
        dbContext.ManageEntities(new ModelBuilder());
        dbContext.SeedData("../../../TestData/Accounts.csv");
        
        var databaseService = new DatabaseService(dbContext, mockLogger.Object);

        // Act
        var idsToCheck = new List<int> { 2344 };
        var result = databaseService.GetIdRecordsWhichHaveProvidedIds(dbContext, idsToCheck);

        // Assert
        Assert.Equal(idsToCheck, result);
    }
    
    [Fact]
    public async Task InsertMeterReadingRecordsAsync_SuccessfulInsertions()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DatabaseService>>();
        
        var meterReadingsToInsert = new List<MeterReading>
        {
            new() { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 100 },
            new() { AccountId = 2, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 200 },
            new() { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 150 },
        };
        
        var options = new DbContextOptionsBuilder<EnsekDbEntity>()
            .UseInMemoryDatabase("TestDb3")
            .Options;
        
        // Act: Create the in-memory database context and add account data, which we'll use to compare
        await using var dbContext = new EnsekDbEntity(options);
        dbContext.ManageEntities(new ModelBuilder());

        var databaseService = new DatabaseService(dbContext, mockLogger.Object);

        var result = await databaseService.InsertMeterReadingRecordsAsync(
            meterReadingsToInsert.GroupBy(m => m.AccountId));

        // Assert
        Assert.Equal(meterReadingsToInsert.Count, result.SuccessfulReadingsCount);
        Assert.Equal(0, result.FailedReadingsCount);
    }
    
    [Fact]
    public async Task InsertMeterReadingRecordsAsync_Some_Successful_Some_UnsuccessfulInsertions()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DatabaseService>>();
        var date = DateTime.UtcNow;
        
        var meterReadingsToInsert = new List<MeterReading>
        {
            new() { AccountId = 1, MeterReadingDateTime = date, MeterReadValue = 100 },
            new() { AccountId = 1, MeterReadingDateTime = date.AddMinutes(1), MeterReadValue = 100 },
            new() { AccountId = 1, MeterReadingDateTime = date.AddMinutes(2), MeterReadValue = 100 },
            new() { AccountId = 1, MeterReadingDateTime = date, MeterReadValue = 101 },
            new() { AccountId = 1, MeterReadingDateTime = date.AddMinutes(2), MeterReadValue = 100 }, // Unsuccessful insertion
        };
        
        var options = new DbContextOptionsBuilder<EnsekDbEntity>()
            .UseInMemoryDatabase("TestDb3")
            .Options;
        
        // Act: Create the in-memory database context and add account data, which we'll use to compare
        await using var dbContext = new EnsekDbEntity(options);
        dbContext.ManageEntities(new ModelBuilder());

        var databaseService = new DatabaseService(dbContext, mockLogger.Object);

        var result = await databaseService.InsertMeterReadingRecordsAsync(
            meterReadingsToInsert.GroupBy(m => m.AccountId));

        // Assert
        Assert.Equal(4, result.SuccessfulReadingsCount);
        Assert.Equal(1, result.FailedReadingsCount);
    }
}