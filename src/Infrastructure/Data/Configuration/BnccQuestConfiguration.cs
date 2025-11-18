using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class BnccQuestConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<BnccQuest>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<BnccQuest> builder)
    {
        builder.HasKey(bq => bq.Id);

        builder.Property(bq => bq.QuestId).IsRequired();
        builder.Property(bq => bq.BnccId).IsRequired();

        builder
            .HasOne(bq => bq.Quest)
            .WithMany(q => q.BnccQuests)
            .HasForeignKey(bq => bq.QuestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(bq => bq.Bncc)
            .WithMany(b => b.BnccQuests)
            .HasForeignKey(bq => bq.BnccId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice composto para evitar duplicatas
        builder.HasIndex(bq => new { bq.QuestId, bq.BnccId }).IsUnique();
    }
}
