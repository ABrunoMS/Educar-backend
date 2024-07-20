using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class GameConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Game>
{
    private readonly DatabaseFacade _database = database;
    
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Name).IsUnique();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Lore).IsRequired();
        builder.Property(t => t.Purpose).IsRequired().HasMaxLength(255);
    }
}