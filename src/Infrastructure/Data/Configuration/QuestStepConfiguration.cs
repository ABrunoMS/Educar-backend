using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestStepConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestStep>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<QuestStep> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Order).IsRequired();
        builder.Property(t => t.NpcType).HasConversion<string>();
        builder.Property(t => t.NpcBehaviour).HasConversion<string>();
        builder.Property(t => t.QuestStepType).HasConversion<string>();
        builder.Property(t => t.QuestId).IsRequired();
        
        builder
            .HasMany(g => g.QuestStepNpcs)
            .WithOne(gs => gs.QuestStep)
            .HasForeignKey(gs => gs.QuestStepId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.QuestStepMedias)
            .WithOne(gs => gs.QuestStep)
            .HasForeignKey(gs => gs.QuestStepId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.QuestStepItems)
            .WithOne(gs => gs.QuestStep)
            .HasForeignKey(gs => gs.QuestStepId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
    }
}
