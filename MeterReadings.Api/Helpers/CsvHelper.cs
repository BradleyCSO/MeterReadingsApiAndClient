using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace EnsekExercise.Helpers;

public static class CsvHelper
{
    /// <summary>
    ///     Provided a path, reads a CSV
    /// </summary>
    /// <param name="path">Path to CSV</param>
    /// <typeparam name="T">Generic: type to bind to</typeparam>
    /// <returns>Enumerable set of results of <typeparam name="T"></typeparam></returns>
    public static IEnumerable<T>? ReadCsvData<T>(string path)
    {
        if (string.IsNullOrEmpty(path))
            return default;
        
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ReadingExceptionOccurred = re => false,
        });

        // Read and process the CSV data
        return csv.GetRecords<T>()?.ToList() ?? [];
    }
    
    /// <summary>
    ///     Allows for validation logic provided a <see cref="ClassMap{TClass}"/>
    /// </summary>
    /// <param name="stream">Stream to pass in: i.e. from request body</param>
    /// <param name="classMap">ClassMap of type T <see cref="Models.MeterReadingMap"/></param>
    /// <typeparam name="T">Generic type of T</typeparam>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ReadCsvDataAsync<T>(Stream? stream, ClassMap<T> classMap) 
        where T : class
    {
        if (stream == null)
            return [];

        using var reader = new StreamReader(stream);
        //var yo = await reader.ReadToEndAsync();
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            ReadingExceptionOccurred = re => false,
        });

        // Register the provided class map: used for validation logic
        csv.Context.RegisterClassMap(classMap);

        var records = new List<T>();
        await foreach (var record in csv.GetRecordsAsync<T>())
        {
            records.Add(record);
        }

        return records;
    }
}