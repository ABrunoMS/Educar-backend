using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class SchoolConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<School>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.ClientId).IsRequired();
        builder.Property(t => t.RegionalId).IsRequired();

        builder.HasOne(s => s.Regional)
            .WithMany(r => r.Schools)
            .HasForeignKey(s => s.RegionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
