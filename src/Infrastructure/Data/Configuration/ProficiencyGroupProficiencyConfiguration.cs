using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ProficiencyGroupProficiencyConfiguration(DatabaseFacade database)
    : IEntityTypeConfiguration<ProficiencyGroupProficiency>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<ProficiencyGroupProficiency> builder)
    {
        builder.HasKey(ac => new { ac.ProficiencyGroupId, ac.ProficiencyId });

        builder.HasOne(ac => ac.ProficiencyGroup)
            .WithMany(a => a.ProficiencyGroupProficiencies)
            .HasForeignKey(ac => ac.ProficiencyGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Proficiency)
            .WithMany(c => c.ProficiencyGroupProficiencies)
            .HasForeignKey(ac => ac.ProficiencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}