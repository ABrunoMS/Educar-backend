using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClassQuestConfiguration : IEntityTypeConfiguration<ClassQuest>
{
    public void Configure(EntityTypeBuilder<ClassQuest> builder)
    {
        // O Id (Guid) de BaseEntity será a primary key
        builder.HasKey(cq => cq.Id);

        // Índice único para evitar duplicação de ClassId + QuestId
        builder.HasIndex(cq => new { cq.ClassId, cq.QuestId })
            .IsUnique();

        builder.HasOne(cq => cq.Class)
            .WithMany(c => c.ClassQuests)
            .HasForeignKey(cq => cq.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cq => cq.Quest)
            .WithMany(q => q.ClassQuests)
            .HasForeignKey(cq => cq.QuestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cq => cq.ExpirationDate)
            .IsRequired();
    }
}
