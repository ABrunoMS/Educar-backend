using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class QuestStepContentConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<QuestStepContent>
{
    public void Configure(EntityTypeBuilder<QuestStepContent> builder)
    {
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.QuestionType).HasConversion<string>();
        builder.Property(t => t.QuestStepContentType).HasConversion<string>();
        builder.Property(t => t.Weight).HasColumnType("decimal(5,2)");
        builder.Property(t => t.QuestStepId).IsRequired();
        builder.ConfigureJsonProperty(nameof(QuestStepContent.ExpectedAnswers), database).IsRequired();
        
        builder
            .HasMany(qsc => qsc.Answers)
            .WithOne(ans => ans.QuestStepContent)
            .HasForeignKey(ans => ans.QuestStepContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
