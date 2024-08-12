using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class NpcConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Npc>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Npc> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Lore).IsRequired();
        builder.Property(t => t.NpcType).IsRequired().HasConversion<string>();
        builder.Property(t => t.GoldDropRate).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(t => t.GoldAmount).IsRequired().HasColumnType("decimal(10,2)");

        builder
            .HasMany(g => g.NpcItems)
            .WithOne(gs => gs.Npc)
            .HasForeignKey(gs => gs.NpcId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.GameNpcs)
            .WithOne(gs => gs.Npc)
            .HasForeignKey(gs => gs.NpcId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(g => g.QuestStepNpcs)
            .WithOne(gs => gs.Npc)
            .HasForeignKey(gs => gs.NpcId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}