using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestStepItemConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestStepItem>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<QuestStepItem> builder)
    {
        builder.HasKey(ac => new { ac.QuestStepId, ac.ItemId });

        builder.HasOne(ac => ac.QuestStep)
            .WithMany(a => a.QuestStepItems)
            .HasForeignKey(ac => ac.QuestStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Item)
            .WithMany(c => c.QuestStepItems)
            .HasForeignKey(ac => ac.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}