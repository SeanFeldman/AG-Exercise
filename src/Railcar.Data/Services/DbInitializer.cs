using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Railcar.Data.Entities;

namespace Railcar.Data.Services;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services, 
        bool executeMigrations = true,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RailcarDbContext>();

        if (executeMigrations)
        {
            await context.Database.MigrateAsync(cancellationToken);
        }

        await SeedCitiesAsync(context, cancellationToken);
        await SeedEventCodesAsync(context, cancellationToken);
    }

    private static async Task SeedCitiesAsync(
        RailcarDbContext context, 
        CancellationToken cancellationToken)
    {
        if (await context.Cities.AnyAsync(cancellationToken))
        {
            return;
        }

        var csvPath = ResolveDataPath("canadian_cities.csv");
        if (!File.Exists(csvPath))
        {
            // TODO: log warning that city CSV seed file is missing.
            return;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<CityCsvMap>();

        var records = new List<City>();

        await foreach (var row in csv.GetRecordsAsync<CityCsv>(cancellationToken))
        {
            records.Add(new City
            {
                Id = row.CityId,
                Name = row.CityName,
                TimeZoneId = row.TimeZone,
            });
        }

        await context.Cities.AddRangeAsync(records, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedEventCodesAsync(
        RailcarDbContext context, 
        CancellationToken cancellationToken)
    {
        if (await context.EventCodeDefinitions.AnyAsync(cancellationToken))
        {
            return;
        }

        var csvPath = ResolveDataPath("event_code_definitions.csv");
        if (!File.Exists(csvPath))
        {
            // TODO: log warning that event code CSV seed file is missing.
            return;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<EventCodeDefinitionCsvMap>();

        var records = new List<EventCodeDefinition>();

        await foreach (var row in csv.GetRecordsAsync<EventCodeDefinitionCsv>(cancellationToken))
        {
            records.Add(new EventCodeDefinition
            {
                Code = row.EventCode,
                Description = row.EventDescription,
                LongDescription = row.LongDescription,
            });
        }

        await context.EventCodeDefinitions.AddRangeAsync(records, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static string ResolveDataPath(
        string fileName)
    {
        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, fileName);
        return Path.GetFullPath(path);
    }
}
