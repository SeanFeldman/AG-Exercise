using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Railcar.Data.Entities;
using Railcar.Shared.Trips;

namespace Railcar.Data.Services;

public sealed class TripImportService(RailcarDbContext dbContext) : ITripImportService
{
    public async Task<IReadOnlyList<RailcarTripDto>> ImportTripsAsync(Stream csvStream, CancellationToken cancellationToken)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<EquipmentEventCsvMap>();

        var rawEvents = new List<EquipmentEventCsv>();

        await foreach (var row in csv.GetRecordsAsync<EquipmentEventCsv>(cancellationToken))
        {
            rawEvents.Add(row);
        }

        if (rawEvents.Count == 0)
        {
            return [];
        }

        var cities = await dbContext.Cities.AsNoTracking().ToListAsync(cancellationToken);
        var cityMap = cities.ToDictionary(c => c.Id, c => c);

        var eventCodeDefinitions = await dbContext.EventCodeDefinitions.AsNoTracking().ToListAsync(cancellationToken);
        var eventCodeSet = eventCodeDefinitions.Select(e => e.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var equipmentEvents = new List<EquipmentEvent>();

        foreach (var e in rawEvents)
        {
            if (!cityMap.TryGetValue(e.CityId, out var city))
            {
                continue;
            }

            if (!eventCodeSet.Contains(e.EventCode))
            {
                continue;
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(city.TimeZoneId);

            var local = DateTime.SpecifyKind(e.EventTime, DateTimeKind.Unspecified);
            if (timeZone.IsInvalidTime(local))
            {
                // TODO: This needs a stricter policy implementation as right now the code correctly handles the DST gap (spring forward) and prevents errors,
                // but it leaves the DST overlap (fall back) to the default BCL behavior.
                // https://www.nuget.org/packages/NodaTime to the rescue
                var adjustmentRule = timeZone.GetAdjustmentRules()
                    .FirstOrDefault(r => r.DateStart <= local.Date && local.Date <= r.DateEnd);

                if (adjustmentRule is not null)
                {
                    local += adjustmentRule.DaylightDelta;
                }
                else
                {
                    local = local.AddHours(1);
                }
            }
            var utc = TimeZoneInfo.ConvertTimeToUtc(local, timeZone);

            equipmentEvents.Add(new EquipmentEvent
            {
                EquipmentId = e.EquipmentId,
                EventCode = e.EventCode,
                CityId = e.CityId,
                EventLocalTime = local,
                EventUtcTime = utc,
            });
        }

        if (equipmentEvents.Count == 0)
        {
            return [];
        }

        var trips = BuildTrips(equipmentEvents);

        await dbContext.Trips.AddRangeAsync(trips, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var tripDtos = trips.Select(x => new RailcarTripDto
        {
            Id = x.Id,
            EquipmentId = x.EquipmentId,
            OriginCityName = cityMap[x.OriginCityId].Name,
            DestinationCityName = cityMap[x.DestinationCityId].Name,
            StartUtc = x.StartUtc,
            EndUtc = x.EndUtc,
            TotalTripHours = x.TotalTripHours,
        }).ToList();

        return tripDtos;
    }

    private static List<Trip> BuildTrips(List<EquipmentEvent> events)
    {
        var result = new List<Trip>();

        var groups = events
            .GroupBy(e => e.EquipmentId, StringComparer.OrdinalIgnoreCase);

        foreach (var group in groups)
        {
            var ordered = group.OrderBy(e => e.EventUtcTime).ToList();

            Trip? currentTrip = null;
            var currentEvents = new List<EquipmentEvent>();

            foreach (var ev in ordered)
            {
                if (string.Equals(ev.EventCode, "W", StringComparison.OrdinalIgnoreCase))
                {
                    currentTrip = new Trip
                    {
                        EquipmentId = ev.EquipmentId,
                        OriginCityId = ev.CityId,
                        StartUtc = ev.EventUtcTime,
                    };

                    currentEvents.Clear();
                    currentEvents.Add(ev);
                    continue;
                }

                if (currentTrip is null)
                {
                    continue;
                }

                currentEvents.Add(ev);

                if (!string.Equals(ev.EventCode, "Z", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                currentTrip.DestinationCityId = ev.CityId;
                currentTrip.EndUtc = ev.EventUtcTime;
                currentTrip.TotalTripHours = (currentTrip.EndUtc - currentTrip.StartUtc).TotalHours;

                foreach (var @event in currentEvents)
                {
                    @event.Trip = currentTrip;
                }

                currentTrip.Events = new List<EquipmentEvent>(currentEvents);

                result.Add(currentTrip);

                currentTrip = null;
                currentEvents.Clear();
            }
        }

        return result;
    }
}
