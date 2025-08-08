using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class AddressConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Address>
{
    private readonly DatabaseFacade _database = database;
    
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Property(t => t.Street).IsRequired().HasMaxLength(255);
        builder.Property(t => t.City).IsRequired().HasMaxLength(100);
        builder.Property(t => t.State).IsRequired().HasMaxLength(100);
        builder.Property(t => t.PostalCode).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Country).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Lat).HasColumnType("decimal(9,6)");
        builder.Property(t => t.Lng).HasColumnType("decimal(9,6)");
    }
}