using Railcar.Shared.Trips;

namespace Railcar.Data.Services;

public interface ITripImportService
{
    Task<IReadOnlyList<RailcarTripDto>> ImportTripsAsync(Stream csvStream, CancellationToken cancellationToken);
}
