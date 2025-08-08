using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ItemConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Item>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Lore).IsRequired();
        builder.Property(t => t.ItemType).IsRequired().HasConversion<string>();
        builder.Property(t => t.ItemRarity).IsRequired().HasConversion<string>();
        builder.Property(t => t.SellValue).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(t => t.Reference2D).IsRequired().HasMaxLength(255);
        builder.Property(t => t.Reference3D).IsRequired().HasMaxLength(255);
        builder.Property(t => t.DropRate).IsRequired().HasColumnType("decimal(5,2)");

        builder
            .HasMany(g => g.NpcItems)
            .WithOne(gs => gs.Item)
            .HasForeignKey(gs => gs.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(g => g.QuestStepItems)
            .WithOne(gs => gs.Item)
            .HasForeignKey(gs => gs.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}