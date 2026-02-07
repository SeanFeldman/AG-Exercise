namespace Railcar.Data.Entities;

public sealed class Trip
{
    public long Id { get; set; }

    public required string EquipmentId { get; set; }

    public int OriginCityId { get; set; }
    public City? OriginCity { get; set; }

    public int DestinationCityId { get; set; }
    public City? DestinationCity { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public double TotalTripHours { get; set; }

    public ICollection<EquipmentEvent> Events { get; set; } = new List<EquipmentEvent>();
}
