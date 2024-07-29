using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ProficiencyConfigutarion(DatabaseFacade database) : IEntityTypeConfiguration<Proficiency>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Proficiency> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Purpose).IsRequired().HasMaxLength(255);

        builder
            .HasMany(a => a.ProficiencyGroupProficiencies)
            .WithOne(c => c.Proficiency)
            .HasForeignKey(c => c.ProficiencyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}