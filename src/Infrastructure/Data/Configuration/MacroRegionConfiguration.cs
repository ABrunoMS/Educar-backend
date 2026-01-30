using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class MacroRegionConfiguration : IEntityTypeConfiguration<MacroRegion>
{
    public void Configure(EntityTypeBuilder<MacroRegion> builder)
    {
        builder.Property(m => m.Name).IsRequired();
    }
}
