using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Railcar.Data.Entities;

namespace Railcar.Data.Configurations;

public sealed class EventCodeDefinitionConfiguration : IEntityTypeConfiguration<EventCodeDefinition>
{
    public void Configure(EntityTypeBuilder<EventCodeDefinition> entity)
    {
        entity.HasKey(e => e.Code);
        entity.Property(e => e.Code).HasMaxLength(8);
        entity.Property(e => e.Description).IsRequired().HasMaxLength(100);
        entity.Property(e => e.LongDescription).IsRequired().HasMaxLength(500);
    }
}
