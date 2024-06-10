using EnsekExercise.DbEntities;
using EnsekExercise.Models;

namespace EnsekExercise.Services.Interfaces;

public interface IDatabaseService
{
    /// <summary>
    ///     Looks up the AccountId provided a List of integers of ids
    /// </summary>
    /// <param name="idsToCheck">The List of integers to lookup the database with</param>
    /// <returns>A list of integers containing the AccountIds where there was a match</returns>
    IEnumerable<int> GetIdRecordsWhichHaveProvidedIds(IEnumerable<int> idsToCheck);
    IEnumerable<int> GetIdRecordsWhichHaveProvidedIds(EnsekDbEntity ensekDbEntity,
        IEnumerable<int> idsToCheck);
    
    Task <(int SuccessfulReadingsCount, int FailedReadingsCount)> InsertMeterReadingRecordsAsync
        (IEnumerable<IGrouping<int, MeterReading>> meterReadingRecordsToInsert);
}