using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Railcar.Data.Entities;

namespace Railcar.Data.Configurations;

public sealed class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> entity)
    {
        entity.HasKey(t => t.Id);
        entity.Property(t => t.EquipmentId).IsRequired().HasMaxLength(50);

        entity.HasOne(t => t.OriginCity)
            .WithMany(c => c.OriginTrips)
            .HasForeignKey(t => t.OriginCityId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(t => t.DestinationCity)
            .WithMany(c => c.DestinationTrips)
            .HasForeignKey(t => t.DestinationCityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
