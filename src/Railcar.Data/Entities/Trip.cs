namespace Railcar.Data.Entities;

public sealed class Trip
{
    public long Id { get; init; }

    public required string EquipmentId { get; init; }

    public int OriginCityId { get; init; }
    public City? OriginCity { get; init; }

    public int DestinationCityId { get; set; }
    public City? DestinationCity { get; init; }

    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; set; }

    public double TotalTripHours { get; set; }

    public ICollection<EquipmentEvent> Events { get; set; } = new List<EquipmentEvent>();
}
