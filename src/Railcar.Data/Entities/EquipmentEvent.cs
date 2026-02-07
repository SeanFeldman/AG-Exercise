namespace Railcar.Data.Entities;

public sealed class EquipmentEvent
{
    public long Id { get; set; }

    public required string EquipmentId { get; set; }

    public required string EventCode { get; set; }
    public EventCodeDefinition? EventCodeDefinition { get; set; }

    public int CityId { get; set; }
    public City? City { get; set; }

    public DateTime EventLocalTime { get; set; }
    public DateTime EventUtcTime { get; set; }

    public long? TripId { get; set; }
    public Trip? Trip { get; set; }
}
