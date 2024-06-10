using EnsekExercise.DbEntities;
using EnsekExercise.Exceptions;
using EnsekExercise.Models;
using EnsekExercise.Services.Interfaces;

namespace EnsekExercise.Services;

/// <summary>
///     Wrapper class for our <see cref="EnsekDbEntity"/> Npgsql Entity Framework Core Database Provider
/// </summary>
/// <param name="dbContext"></param>
public class DatabaseService(EnsekDbEntity dbContext, ILogger<DatabaseService> logger) : IDatabaseService
{
    public IEnumerable<int> GetIdRecordsWhichHaveProvidedIds(IEnumerable<int> idsToCheck) => 
        dbContext.Accounts
            .Where(a => idsToCheck.Contains(a.AccountId))
            .Select(a => a.AccountId)
            .ToList();

    public IEnumerable<int> GetIdRecordsWhichHaveProvidedIds(EnsekDbEntity ensekDbEntity,
        IEnumerable<int> idsToCheck) => 
        ensekDbEntity.Accounts
            .Where(a => idsToCheck.Contains(a.AccountId))
            .Select(a => a.AccountId)
            .ToList();

    /// <summary>
    ///     Individually go through each validated CSV entry: if it already exists, don't add else insert to table
    ///     i.e. partial writes
    /// </summary>
    /// <param name="meterReadingRecordsToInsert">The meter reading records from our CSV</param>
    /// <returns>Integer: count of the number of records which were inserted</returns>
    public async Task<(int SuccessfulReadingsCount, int FailedReadingsCount)> InsertMeterReadingRecordsAsync(IEnumerable<IGrouping<int, MeterReading>> meterReadingRecordsToInsert)
    {
        (int SuccessfulReadingsCount, int FailedReadingsCount) readings = new();

        try
        {
            // Explored 'grouping' records, in a way that could be used to display meter readings and account for client
            foreach (var group in meterReadingRecordsToInsert)
            {
                foreach (var meterReading in group)
                {
                    try
                    {
                        // Convert our DateTime type to the kind that PostreSQL likes
                        meterReading.MeterReadingDateTime =
                            DateTime.SpecifyKind(meterReading.MeterReadingDateTime, DateTimeKind.Utc);
                        
                        await dbContext.MeterReadings.AddAsync(meterReading);
                        
                        // Insert record one by one: ensure uniqueness
                        await dbContext.SaveChangesAsync();
                        
                        readings.SuccessfulReadingsCount++;
                    }
                    catch
                    {
                        // Increment the count of failed readings, but continue enumeration: likely a pre-existing record
                        readings.FailedReadingsCount++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error when inserting records");
            
            // Throw back to the API so that we don't unnecessarily expose information about our DatabaseService,
            // and so that the API can handle it in a graceful way i.e. different status code
            throw new FailedToInsertRecordsException("There was an error inserting records.");
        }

        return readings;
    }
}