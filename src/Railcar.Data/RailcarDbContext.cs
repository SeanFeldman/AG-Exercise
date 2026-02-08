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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RailcarDbContext).Assembly);
    }
}
