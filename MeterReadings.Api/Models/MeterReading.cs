using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace EnsekExercise.Models;

public class MeterReading
{
    public int AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; } 
    public int MeterReadValue { get; set; }
}

public class MeterReadingMap : ClassMap<MeterReading>
{
    public MeterReadingMap()
    {
        Map(m => m.AccountId);
        Map(m => m.MeterReadingDateTime).TypeConverter<DateTimeConverter>()
            .TypeConverterOption.Format("dd/MM/yyyy HH:mm");
        Map(m => m.MeterReadValue)
            .Validate(field =>
            {
                if (int.TryParse(field.Field, out int numericValue))
                    return numericValue is >= 0 and <= 99999;

                return false;
            });
    }
}