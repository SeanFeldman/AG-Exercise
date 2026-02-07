namespace Railcar.Data.Entities;

public sealed class EventCodeDefinition
{
    public required string Code { get; set; }
    public required string Description { get; set; }
    public required string LongDescription { get; set; }

    public ICollection<EquipmentEvent> EquipmentEvents { get; set; } = new List<EquipmentEvent>();
}
