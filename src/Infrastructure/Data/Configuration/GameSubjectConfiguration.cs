using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class GameSubjectConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<GameSubject>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<GameSubject> builder)
    {
        builder.HasKey(ac => new { ac.GameId, ac.SubjectId });

        builder.HasOne(ac => ac.Game)
            .WithMany(a => a.GameSubjects)
            .HasForeignKey(ac => ac.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Subject)
            .WithMany(c => c.GameSubjects)
            .HasForeignKey(ac => ac.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}