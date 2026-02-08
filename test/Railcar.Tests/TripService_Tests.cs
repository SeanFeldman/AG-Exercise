using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Railcar.Data;
using Railcar.Data.Entities;
using Railcar.Data.Services;
using Shouldly;

namespace Railcar.Tests;

public sealed class TripService_Tests
{
    [Fact]
    public async Task GetTripEventsAsync_Returns_events_in_expected_order_and_shape()
    {
        const string dbName = "TripServiceTests_" + nameof(GetTripEventsAsync_Returns_events_in_expected_order_and_shape);

        var services = new ServiceCollection()
            .AddDbContext<RailcarDbContext>(builder => builder.UseInMemoryDatabase(dbName))
            .BuildServiceProvider();

        await using var dbContext = services.GetRequiredService<RailcarDbContext>();

        var trip = await SeedTestData(dbContext);
        var service = new TripService(dbContext);

        var result = await service.GetTripEventsAsync(trip.Id, CancellationToken.None);

        result.Count.ShouldBe(4);

        result.Select(e => e.EventUtcTime).ShouldBeInOrder("Should be ordered by event date/time");

        result[0].EventDescription.ShouldBe("Released", "Should starts with Released (W)");
        result[^1].EventDescription.ShouldBe("Placed", "Should ends with Placed (Z)");
    }

    private static async Task<Trip> SeedTestData(RailcarDbContext dbContext)
    {
        var cityOrigin = new City { Id = 1, Name = "Calgary", TimeZoneId = "Mountain Standard Time" };
        var cityDestination = new City { Id = 2, Name = "Edmonton", TimeZoneId = "Mountain Standard Time" };

        await dbContext.Cities.AddRangeAsync(cityOrigin, cityDestination);

        var codeW = new EventCodeDefinition { Code = "W", Description = "Released", LongDescription = "Released" };
        var codeZ = new EventCodeDefinition { Code = "Z", Description = "Placed", LongDescription = "Placed" };
        var codeA = new EventCodeDefinition { Code = "A", Description = "Other A", LongDescription = "Other A" };
        var codeD = new EventCodeDefinition { Code = "D", Description = "Other D", LongDescription = "Other D" };

        await dbContext.EventCodeDefinitions.AddRangeAsync(codeW, codeZ, codeA, codeD);

        var trip = new Trip
        {
            EquipmentId = "CAR-1",
            OriginCityId = cityOrigin.Id,
            DestinationCityId = cityDestination.Id,
            StartUtc = new DateTime(2025, 01, 01, 10, 00, 00, DateTimeKind.Utc),
            EndUtc = new DateTime(2025, 01, 01, 15, 00, 00, DateTimeKind.Utc),
            TotalTripHours = 5,
        };

        await dbContext.Trips.AddAsync(trip);
        await dbContext.SaveChangesAsync();

        var events = new List<EquipmentEvent>
        {
            // Out of order on purpose: Z comes first by insertion but last by time
            new()
            {
                TripId = trip.Id,
                EquipmentId = trip.EquipmentId,
                CityId = cityDestination.Id,
                EventCode = "Z",
                EventLocalTime = new DateTime(2025, 01, 01, 13, 00, 00),
                EventUtcTime = new DateTime(2025, 01, 01, 16, 00, 00, DateTimeKind.Utc),
                City = cityDestination,
                EventCodeDefinition = codeZ,
            },
            // W is the earliest event
            new()
            {
                TripId = trip.Id,
                EquipmentId = trip.EquipmentId,
                CityId = cityOrigin.Id,
                EventCode = "W",
                EventLocalTime = new DateTime(2025, 01, 01, 08, 00, 00),
                EventUtcTime = new DateTime(2025, 01, 01, 11, 00, 00, DateTimeKind.Utc),
                City = cityOrigin,
                EventCodeDefinition = codeW,
            },
            // Mid-trip event at origin
            new()
            {
                TripId = trip.Id,
                EquipmentId = trip.EquipmentId,
                CityId = cityOrigin.Id,
                EventCode = "A",
                EventLocalTime = new DateTime(2025, 01, 01, 09, 30, 00),
                EventUtcTime = new DateTime(2025, 01, 01, 12, 30, 00, DateTimeKind.Utc),
                City = cityOrigin,
                EventCodeDefinition = codeA,
            },
            // Mid-trip event at destination
            new()
            {
                TripId = trip.Id,
                EquipmentId = trip.EquipmentId,
                CityId = cityDestination.Id,
                EventCode = "D",
                EventLocalTime = new DateTime(2025, 01, 01, 11, 30, 00),
                EventUtcTime = new DateTime(2025, 01, 01, 14, 30, 00, DateTimeKind.Utc),
                City = cityDestination,
                EventCodeDefinition = codeD,
            },
        };

        await dbContext.EquipmentEvents.AddRangeAsync(events);
        await dbContext.SaveChangesAsync();

        return trip;
    }
}
