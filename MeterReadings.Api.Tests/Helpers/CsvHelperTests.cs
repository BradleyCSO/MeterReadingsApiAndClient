using CsvHelper;
using EnsekExercise.Models;
using static EnsekExercise.Helpers.CsvHelper;

namespace MeterReadingsApi.Tests.Helpers;

public class CsvHelperTests
{
    [Fact]
    public void ReadCsvData_Should_Return_Records()
    {
        // Arrange
        var records = new List<Account>
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

        const string path = "../../../TestData/Accounts.csv";
        
        // Act
        var result = ReadCsvData<Account>(path);

        // Assert
        Assert.Equivalent(records, result);
    }
    
    [Fact]
    public void ReadCsvData_Empty_Path_Should_Return_No_Records()
    {
        // Arrange
        const string path = "";
        
        // Act
        var result = ReadCsvData<Account>(path);

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void ReadCsvData_Csv_Exists_But_Is_Invalid()
    {
        // Arrange
        const string path =  "../../../TestData/Invalid_Csv.csv";
        
        // Act
        var result = ReadCsvData<Account>(path);

        // Assert
        Assert.Empty(result!);
    }
    
    [Fact]
    public void ReadCsvData_Csv_Exists_And_Has_Unexpected_Headers_Relative_To_Bound_Type()
    {
        // Arrange
        const string path =  "../../../TestData/Accounts.csv";

        // Act & Assert
        Assert.Throws<HeaderValidationException>(() => // To ensure that the helper class remains stateless i.e. no injection of logger: we can handle this exception from the function caller
        {
            ReadCsvData<MeterReading>(path);
        }); 
    }
    
    [Fact]
    public async void ClassMap_ReadCsvData_Should_Return_Records()
    {
        // Arrange
        var records = new List<MeterReading>
        {
            new()
            {
                AccountId = 2344,
                MeterReadingDateTime = DateTime.ParseExact("22/04/2019 09:24", "dd/MM/yyyy HH:mm", 
                    System.Globalization.CultureInfo.InvariantCulture),
                MeterReadValue = 01002
            },
            new() 
            {
                AccountId = 2233,
                MeterReadingDateTime = DateTime.ParseExact("22/04/2019 12:25", "dd/MM/yyyy HH:mm", 
                    System.Globalization.CultureInfo.InvariantCulture),
                MeterReadValue = 00323
            },
            new() 
            {
                AccountId = 8766,
                MeterReadingDateTime = DateTime.ParseExact("22/04/2019 12:25", "dd/MM/yyyy HH:mm", 
                    System.Globalization.CultureInfo.InvariantCulture),                
                MeterReadValue = 03440
            }
        };

        const string path = "../../../TestData/Meter_Reading.csv";
        byte[] fileBytes = await File.ReadAllBytesAsync(path);
        MemoryStream stream = new MemoryStream(fileBytes);
        
        // Act
        var result = await ReadCsvDataAsync(stream, new MeterReadingMap());

        // Assert
        Assert.Equivalent(records, result);
    }
    
    [Fact]
    public async void ClassMap_ReadCsvData_Empty_Path_Should_Return_No_Records()
    {
        // Arrange
        const string path =  "../../../TestData/Invalid_Csv.csv";
        byte[] fileBytes = await File.ReadAllBytesAsync(path);
        MemoryStream stream = new MemoryStream(fileBytes);
        
        // Act
        var result = await ReadCsvDataAsync(stream, new MeterReadingMap());

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async void ClassMap_ReadCsvData_Csv_Exists_But_Is_Invalid()
    {
        // Arrange
        const string path =  "../../../TestData/Invalid_Csv.csv";
        byte[] fileBytes = await File.ReadAllBytesAsync(path);
        MemoryStream stream = new MemoryStream(fileBytes);
        
        // Act
        var result = await ReadCsvDataAsync(stream, new MeterReadingMap());

        // Assert
        Assert.Empty(result!);
    }
    
    [Fact]
    public void ClassMap_ReadCsvData_Csv_Exists_And_Has_Unexpected_Headers_Relative_To_Bound_Type()
    {
        // Arrange
        const string path =  "../../../TestData/Meter_Reading.csv";

        // Act & Assert
        Assert.Throws<HeaderValidationException>(() => // To ensure that the helper class remains stateless i.e. no injection of logger: we can handle this exception from the function caller
        {
            ReadCsvData<Account>(path);
        }); 
    }
}