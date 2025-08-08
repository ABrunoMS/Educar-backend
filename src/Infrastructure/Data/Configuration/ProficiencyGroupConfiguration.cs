using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ProficiencyGroupConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<ProficiencyGroup>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<ProficiencyGroup> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired();

        builder
            .HasMany(a => a.ProficiencyGroupProficiencies)
            .WithOne(c => c.ProficiencyGroup)
            .HasForeignKey(c => c.ProficiencyGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.GameProficiencyGroups)
            .WithOne(gs => gs.ProficiencyGroup)
            .HasForeignKey(gs => gs.ProficiencyGroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}