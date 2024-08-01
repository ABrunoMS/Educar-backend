using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class NpcItemConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<NpcItem>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<NpcItem> builder)
    {
        builder.HasKey(ac => new { ac.NpcId, ac.ItemId });

        builder.HasOne(ac => ac.Npc)
            .WithMany(a => a.NpcItems)
            .HasForeignKey(ac => ac.NpcId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Item)
            .WithMany(c => c.NpcItems)
            .HasForeignKey(ac => ac.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}