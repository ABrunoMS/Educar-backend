using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class AnswerConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.Property(t => t.IsCorrect).IsRequired();
        builder.Property(t => t.QuestStepContentId).IsRequired();
        builder.Property(t => t.AccountId).IsRequired();
        builder.ConfigureJsonProperty(nameof(Answer.GivenAnswer), database).IsRequired();
        
        builder
            .HasOne(ans => ans.Account)
            .WithMany(acc => acc.Answers)
            .HasForeignKey(ans => ans.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(ans => ans.QuestStepContent)
            .WithMany(qsc => qsc.Answers)
            .HasForeignKey(t => t.QuestStepContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}