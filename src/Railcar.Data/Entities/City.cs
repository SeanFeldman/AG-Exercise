namespace Railcar.Data.Entities;

public sealed class City
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TimeZoneId { get; set; }

    public ICollection<EquipmentEvent> EquipmentEvents { get; set; } = new List<EquipmentEvent>();
    public ICollection<Trip> OriginTrips { get; set; } = new List<Trip>();
    public ICollection<Trip> DestinationTrips { get; set; } = new List<Trip>();
}
