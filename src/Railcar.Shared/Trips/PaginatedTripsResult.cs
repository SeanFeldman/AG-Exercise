namespace Railcar.Shared.Trips;

public sealed class PaginatedTripsResult
{
    public int PageNumber { get; init; }
    public int PageSize  { get; init; }
    public int TotalCount { get; init; }
    public IReadOnlyList<RailcarTripDto> Items { get; init; } = [];
}
