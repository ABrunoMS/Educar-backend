using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestProficiencyConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestProficiency>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<QuestProficiency> builder)
    {
        builder.HasKey(ac => new { ac.QuestId, ac.ProficiencyId });

        builder.HasOne(ac => ac.Quest)
            .WithMany(a => a.QuestProficiencies)
            .HasForeignKey(ac => ac.QuestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Proficiency)
            .WithMany(c => c.QuestProficiencies)
            .HasForeignKey(ac => ac.ProficiencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}