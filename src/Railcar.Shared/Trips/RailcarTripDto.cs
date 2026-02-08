namespace Railcar.Shared.Trips;

public sealed class RailcarTripDto
{
    public long Id { get; init; }

    public required string EquipmentId { get; init; }
    public required string OriginCityName { get; init; }
    public required string DestinationCityName { get; init; }

    public required DateTime StartUtc { get; init; }
    public required DateTime EndUtc   { get; init; }

    public double TotalTripHours { get; init; }
}
