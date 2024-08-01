using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class GameNpcConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<GameNpc>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<GameNpc> builder)
    {
        builder.HasKey(ac => new { ac.GameId, ac.NpcId });

        builder.HasOne(ac => ac.Game)
            .WithMany(a => a.GameNpcs)
            .HasForeignKey(ac => ac.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Npc)
            .WithMany(c => c.GameNpcs)
            .HasForeignKey(ac => ac.NpcId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}