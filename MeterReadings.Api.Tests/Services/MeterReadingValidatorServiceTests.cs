using EnsekExercise.Services;

namespace MeterReadingsApi.Tests.Services;

public class MeterReadingValidatorServiceTests
{
    [Fact]
    public async Task ValidatedCsvRecordsAsync_Should_Return_Records_From_File()
    {
        const string path = "../../../TestData/Meter_Reading.csv";

        // Arrange
        await using var stream = File.OpenRead(path);
        var service = new MeterReadingValidatorService();

        // Act
        var result = await service.ValidatedCsvRecordsAsync(stream);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count() == 3);
    }
    
    [Fact]
    public async Task ValidatedCsvRecordsAsync_Empty_File()
    {
        const string path = "../../../TestData/Invalid_Csv.csv";

        // Arrange
        await using var stream = File.OpenRead(path);
        var service = new MeterReadingValidatorService();

        // Act
        var result = await service.ValidatedCsvRecordsAsync(stream);

        // Assert
        Assert.Null(result); // Graceful handling
    }
}