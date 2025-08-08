using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class GameProficiencyGroupConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<GameProficiencyGroup>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<GameProficiencyGroup> builder)
    {
        builder.HasKey(ac => new { ac.GameId, ac.ProficiencyGroupId });

        builder.HasOne(ac => ac.Game)
            .WithMany(a => a.GameProficiencyGroups)
            .HasForeignKey(ac => ac.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.ProficiencyGroup)
            .WithMany(c => c.GameProficiencyGroups)
            .HasForeignKey(ac => ac.ProficiencyGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}