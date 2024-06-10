using EnsekExercise.DbEntities;
using EnsekExercise.Exceptions;
using EnsekExercise.Services;
using EnsekExercise.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EnsekDbEntity>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IMeterReadingValidatorService, MeterReadingValidatorService>();
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meter Reading Api",
        Version = "v1"
    });
});
builder.Services.AddCors();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Meter Reading Api");
    options.RoutePrefix = "";
});
app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EnsekDbEntity>();
    context.Database.EnsureCreated();
    context.SeedData("SeedCsvData/Test_Accounts 2.csv");
}

app.MapPost("/meter-reading-uploads", async(
        HttpRequest request,
        [FromServices] IMeterReadingValidatorService meterReadingValidatorService, 
        [FromServices] IDatabaseService databaseService) =>
{
    (int SuccessfulReadingsCount, int FailedReadingsCount) recordsInserted = new();
    // Get validated records from the CSV as a stream: Did try to use https://github.com/dotnet/aspnetcore/issues/47526#issuecomment-1495339058 and https://stackoverflow.com/questions/71499435/how-do-i-do-file-upload-using-asp-net-core-6-minimal-api
    // but seems that Minimal APIs are having troubles with this currently
    var validatedCsvRecords = await meterReadingValidatorService.ValidatedCsvRecordsAsync(request.Body);

    // Query and return which of these initially validated meter reading records have a corresponding AccountId in the Accounts table
    if (validatedCsvRecords.Any())
    {
        // Get the AccountIds from these records  
        var csvAccountIdsToCheck = validatedCsvRecords
            .Select(d => d.Key);
    
        // Query the Accounts table to see if they also have these ids
        var idsWithAssociatedAccountFromDatabase = databaseService.GetIdRecordsWhichHaveProvidedIds(csvAccountIdsToCheck);

        // If these validated meter reading records also have a corresponding AccountId in the Accounts table, try insert them into the MeterReadings table
        if (idsWithAssociatedAccountFromDatabase.Any())
        {
            // Modify the initially validatedCsvRecords to just include ones which have corresponding accounts from the Accounts table
            var meterReadingRecordsToInsert = validatedCsvRecords
                .Where(d=> idsWithAssociatedAccountFromDatabase
                    .Contains(d.Key));
            
            // If there are records worthy of trying to insert, try doing so to the MeterReadings table 
            if (meterReadingRecordsToInsert.Any())
            {
                try
                {
                    recordsInserted = await databaseService.InsertMeterReadingRecordsAsync(meterReadingRecordsToInsert);

                    return Results.Ok(new
                    {
                        recordsInserted.FailedReadingsCount,
                        recordsInserted.SuccessfulReadingsCount,
                    });
                }
                catch (FailedToInsertRecordsException)
                {
                    return Results.BadRequest("Error when trying to insert records. " +
                                              "Please ensure the .csv meets the expected format.");
                }
            }
        }
    }    

    return Results.UnprocessableEntity(new
    {
        recordsInserted.FailedReadingsCount,
        recordsInserted.SuccessfulReadingsCount,
    });
}).DisableAntiforgery().Accepts<IFormFile>("text/plain")
    .Produces(200).Produces(422)
    .WithOpenApi();

app.Run();