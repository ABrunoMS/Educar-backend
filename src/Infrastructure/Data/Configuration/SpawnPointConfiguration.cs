using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class SpawnPointConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<SpawnPoint>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<SpawnPoint> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(150);
        builder.Property(t => t.Reference).IsRequired();
        builder.Property(t => t.X).IsRequired();
        builder.Property(t => t.Y).IsRequired();
        builder.Property(t => t.Z).IsRequired();
        builder.Property(t => t.MapId).IsRequired();
    }
}