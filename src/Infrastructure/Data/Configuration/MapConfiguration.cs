using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class MapConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Map>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Map> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Type).IsRequired().HasConversion<string>();
        builder.Property(t => t.GameId).IsRequired();
        builder.Property(t => t.Reference2D).IsRequired();
        builder.Property(t => t.Reference3D).IsRequired();

        builder
            .HasMany(a => a.SpawnPoints)
            .WithOne(c => c.Map)
            .HasForeignKey(c => c.MapId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}