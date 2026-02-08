using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Railcar.Data;
using Railcar.Data.Services;
using Railcar.Shared.Trips;

namespace Railcar.Tests;

public sealed class TripImportService_Tests
{
    [Fact]
    public async Task Imports_trips_and_returns_railcar_events()
    {
        const string dbName = "TripImportServiceTests_" + nameof(Imports_trips_and_returns_railcar_events);

        var services = new ServiceCollection()
            .AddDbContext<RailcarDbContext>(builder => builder.UseInMemoryDatabase(dbName))
            .BuildServiceProvider();

        await DbInitializer.InitializeAsync(services, executeMigrations: false);

        await using var dbContext = services.GetRequiredService<RailcarDbContext>();

        await using var stream = File.OpenRead("equipment_events.csv");

        var service = new TripImportService(dbContext);

        var result = await service.ImportTripsAsync(stream, CancellationToken.None);

        await Verify(ToSerializableTrips(result))
            .DontScrubDateTimes();
    }

    private static List<object> ToSerializableTrips(IReadOnlyList<RailcarTripDto> trips)
    {
        var list = new List<object>(trips.Count);

        foreach (var t in trips)
        {
            list.Add(new
            {
                t.Id,
                t.EquipmentId,
                t.OriginCityName,
                t.DestinationCityName,
                StartUtc = t.StartUtc.ToString("O"),
                EndUtc = t.EndUtc.ToString("O"),
                t.TotalTripHours
            });
        }

        return list;
    }
}
