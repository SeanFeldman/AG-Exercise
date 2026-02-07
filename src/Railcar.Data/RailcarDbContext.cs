using Microsoft.EntityFrameworkCore;
using Railcar.Data.Entities;

namespace Railcar.Data;

public sealed class RailcarDbContext(DbContextOptions<RailcarDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<EventCodeDefinition> EventCodeDefinitions => Set<EventCodeDefinition>();
    public DbSet<EquipmentEvent> EquipmentEvents => Set<EquipmentEvent>();
    public DbSet<Trip> Trips => Set<Trip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.TimeZoneId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<EventCodeDefinition>(entity =>
        {
            entity.HasKey(e => e.Code);
            entity.Property(e => e.Code).HasMaxLength(8);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LongDescription).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<EquipmentEvent>(entity =>
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
        });

        modelBuilder.Entity<Trip>(entity =>
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
        });
    }
}
