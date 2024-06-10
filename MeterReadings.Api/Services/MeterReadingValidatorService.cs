using EnsekExercise.Services.Interfaces;
using EnsekExercise.Models;
using static EnsekExercise.Helpers.CsvHelper;

namespace EnsekExercise.Services;

public class MeterReadingValidatorService : IMeterReadingValidatorService
{
    public async Task<IEnumerable<IGrouping<int, MeterReading>>> ValidatedCsvRecordsAsync(Stream stream)
    {
        var csvRecords = await ReadCsvDataAsync(stream, new MeterReadingMap());

        if (!csvRecords.Any())
            return null; // We have an empty CSV

        return GroupMeterReadingsByAccountId(csvRecords);
    }
    
    /// <summary>
    ///     Groups each reading by <see cref="Models.MeterReading.AccountId"/> where
    ///     an account can have multiple readings assuming <see cref="Models.MeterReading.MeterReadingDateTime"/>
    ///     <seealso cref="Models.MeterReading.MeterReadValue"/> are both unique
    /// </summary>
    /// <param name="filteredReadings">The readings to filter</param>
    /// <returns>An enumerable set of unique meter readings, grouped by a corresponding AccountId</returns>
    private IEnumerable<IGrouping<int, MeterReading>> GroupMeterReadingsByAccountId(IEnumerable<MeterReading> filteredReadings) => 
        from p in filteredReadings
        group p by p.AccountId into g
        select g;
}