using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestStepNpcConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestStepNpc>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<QuestStepNpc> builder)
    {
        builder.HasKey(ac => new { ac.QuestStepId, ac.NpcId });

        builder.HasOne(ac => ac.QuestStep)
            .WithMany(a => a.QuestStepNpcs)
            .HasForeignKey(ac => ac.QuestStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Npc)
            .WithMany(c => c.QuestStepNpcs)
            .HasForeignKey(ac => ac.NpcId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}