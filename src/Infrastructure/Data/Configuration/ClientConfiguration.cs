using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClientConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Client>
{
    private readonly DatabaseFacade _database = database;
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.Property(t => t.Name).IsRequired();

        builder.HasMany(c => c.ClientProducts)
            .WithOne(cp => cp.Client)
            .HasForeignKey(cp => cp.ClientId);

        builder.HasMany(c => c.ClientContents)
            .WithOne(cc => cc.Client)
            .HasForeignKey(cc => cc.ClientId);

        builder.HasMany(c => c.Subsecretarias)
            .WithOne(s => s.Client)
            .HasForeignKey(s => s.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.MacroRegion)
            .WithMany(m => m.Clients)
            .HasForeignKey(c => c.MacroRegionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
