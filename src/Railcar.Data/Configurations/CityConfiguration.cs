using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Railcar.Data.Entities;

namespace Railcar.Data.Configurations;

public sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> entity)
    {
        entity.HasKey(c => c.Id);
        entity.Property(c => c.Id).ValueGeneratedNever();
        entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
        entity.Property(c => c.TimeZoneId).IsRequired().HasMaxLength(100);
    }
}
