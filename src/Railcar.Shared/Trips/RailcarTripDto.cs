namespace Railcar.Shared.Trips;

public sealed class RailcarTripDto
{
    public required string EquipmentId { get; init; }
    public required int OriginCityId { get; init; }
    public required int DestinationCityId { get; init; }

    public required DateTime StartLocal { get; init; }
    public required DateTime EndLocal   { get; init; }

    public double TotalTripHours => (EndLocal - StartLocal).TotalHours;
}
