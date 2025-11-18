using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class BnccConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Bncc>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Bncc> builder)
    {
        builder.Property(t => t.Description).IsRequired().HasMaxLength(2000); 
        builder.Property(t => t.IsActive).IsRequired();

        builder
            .HasMany(b => b.BnccQuests)
            .WithOne(bq => bq.Bncc)
            .HasForeignKey(bq => bq.BnccId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
