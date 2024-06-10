using EnsekExercise.Models;

namespace EnsekExercise.Services.Interfaces;

public interface IMeterReadingValidatorService
{
    /// <summary>
    ///     Initial validation for the CSV determined by <see cref="MeterReadingMap"/> 
    /// </summary>
    /// <param name="stream">File contents</param>
    public Task<IEnumerable<IGrouping<int, MeterReading>>> ValidatedCsvRecordsAsync(Stream stream);
}