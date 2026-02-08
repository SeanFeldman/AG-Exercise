using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Railcar.Data.Entities;

namespace Railcar.Data.Configurations;

public sealed class EquipmentEventConfiguration : IEntityTypeConfiguration<EquipmentEvent>
{
    public void Configure(EntityTypeBuilder<EquipmentEvent> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.EquipmentId).IsRequired().HasMaxLength(50);
        entity.Property(e => e.EventCode).IsRequired().HasMaxLength(8);

        entity.HasOne(e => e.EventCodeDefinition)
            .WithMany(d => d.EquipmentEvents)
            .HasForeignKey(e => e.EventCode);

        entity.HasOne(e => e.City)
            .WithMany(c => c.EquipmentEvents)
            .HasForeignKey(e => e.CityId);

        entity.HasOne(e => e.Trip)
            .WithMany(t => t.Events)
            .HasForeignKey(e => e.TripId);
    }
}
