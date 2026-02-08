using Microsoft.EntityFrameworkCore;
using Railcar.Shared.Trips;

namespace Railcar.Data.Services;

public sealed class TripService(RailcarDbContext dbContext) : ITripService
{
    public async Task<PaginatedTripsResult> GetTripsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = dbContext.Trips.AsNoTracking();
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .OrderBy(t => t.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new RailcarTripDto
            {
                Id = t.Id,
                EquipmentId = t.EquipmentId,
                OriginCityName = t.OriginCity!.Name,
                DestinationCityName = t.DestinationCity!.Name,
                StartUtc = t.StartUtc,
                EndUtc = t.EndUtc,
                TotalTripHours = t.TotalTripHours,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedTripsResult
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items,
        };
    }

    public async Task<RailcarTripDto?> GetTripByIdAsync(
        long id, 
        CancellationToken cancellationToken)
    {
        return await dbContext.Trips
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new RailcarTripDto
            {
                Id = t.Id,
                EquipmentId = t.EquipmentId,
                OriginCityName = t.OriginCity!.Name,
                DestinationCityName = t.DestinationCity!.Name,
                StartUtc = t.StartUtc,
                EndUtc = t.EndUtc,
                TotalTripHours = t.TotalTripHours,
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RailcarTripEventDto>> GetTripEventsAsync(long tripId, CancellationToken cancellationToken)
    {
        return await dbContext.EquipmentEvents
            .AsNoTracking()
            .Where(e => e.TripId == tripId)
            .OrderBy(e => e.EventUtcTime)
            .Select(e => new RailcarTripEventDto
            {
                Id = e.Id,
                EventDescription = e.EventCodeDefinition!.Description,
                CityName = e.City!.Name,
                EventLocalTime = e.EventLocalTime,
                EventUtcTime = e.EventUtcTime,
            })
            .ToListAsync(cancellationToken);
    }
}
