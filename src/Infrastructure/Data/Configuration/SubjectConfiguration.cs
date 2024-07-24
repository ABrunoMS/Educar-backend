using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class SubjectConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Subject>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired();

        builder
            .HasMany(a => a.GameSubjects)
            .WithOne(c => c.Subject)
            .HasForeignKey(c => c.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}