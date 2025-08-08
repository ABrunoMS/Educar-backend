using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Quest>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Quest> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.UsageTemplate).IsRequired().HasConversion<string>();
        builder.Property(t => t.Type).IsRequired().HasConversion<string>();
        builder.Property(t => t.MaxPlayers).IsRequired();
        builder.Property(t => t.TotalQuestSteps).IsRequired();
        builder.Property(t => t.CombatDifficulty).IsRequired().HasConversion<string>();
        builder.Property(t => t.GameId).IsRequired();
        builder.Property(t => t.SubjectId).IsRequired();
        builder.Property(t => t.GradeId).IsRequired();

        builder
            .HasMany(a => a.QuestProficiencies)
            .WithOne(c => c.Quest)
            .HasForeignKey(c => c.QuestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(q => q.QuestSteps)
            .WithOne(qs => qs.Quest)
            .HasForeignKey(qs => qs.QuestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}