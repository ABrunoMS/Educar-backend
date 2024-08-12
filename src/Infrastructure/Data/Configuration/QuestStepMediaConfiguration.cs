using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestStepMediaConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestStepMedia>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<QuestStepMedia> builder)
    {
        builder.HasKey(ac => new { ac.QuestStepId, ac.MediaId });

        builder.HasOne(ac => ac.QuestStep)
            .WithMany(a => a.QuestStepMedias)
            .HasForeignKey(ac => ac.QuestStepId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Media)
            .WithMany(c => c.QuestStepMedias)
            .HasForeignKey(ac => ac.MediaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}