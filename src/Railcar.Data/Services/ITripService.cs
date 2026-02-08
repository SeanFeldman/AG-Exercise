using Railcar.Shared.Trips;

namespace Railcar.Data.Services;

public interface ITripService
{
    Task<PaginatedTripsResult> GetTripsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<RailcarTripDto?> GetTripByIdAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<RailcarTripEventDto>> GetTripEventsAsync(long tripId, CancellationToken cancellationToken);
}
