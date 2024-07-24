using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClassConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Class>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.Purpose).IsRequired().HasConversion<string>();
        builder.Property(t => t.SchoolId).IsRequired();

        builder
            .HasMany(a => a.AccountClasses)
            .WithOne(c => c.Class)
            .HasForeignKey(c => c.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}