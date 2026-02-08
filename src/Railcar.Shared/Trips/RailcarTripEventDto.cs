namespace Railcar.Shared.Trips;

public sealed class RailcarTripEventDto
{
    public long Id { get; init; }

    public required string EventDescription { get; init; }
    public required string CityName { get; init; }

    public DateTime EventLocalTime { get; init; }
    public DateTime EventUtcTime { get; init; }
}
