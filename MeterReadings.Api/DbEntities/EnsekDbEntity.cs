using EnsekExercise.Models;
using Microsoft.EntityFrameworkCore;
using static EnsekExercise.Helpers.CsvHelper;

namespace EnsekExercise.DbEntities;

public class EnsekDbEntity(DbContextOptions<EnsekDbEntity> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ManageEntities(modelBuilder);
    }

    public void ManageEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .ToTable("Accounts");

        // Define the composite key: ensures no duplicate entries i.e.
        // the same account ID with two different meter readings constitutes two entries
        modelBuilder.Entity<MeterReading>()
            .ToTable("MeterReadings")
            .HasKey(e => new { e.AccountId, e.MeterReadingDateTime, e.MeterReadValue }); 
    }

    public void SeedData(string path)
    {
        try
        {
            // Read data from CSV and add it to the Accounts table
            var accountsData = ReadCsvData<Account>(path);
            Accounts.AddRange(accountsData);
            SaveChanges();
        }
        catch
        {
            // Data already seeded
        }
    }
}