using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class GradeConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Grade>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired();

        builder
            .HasMany(g => g.Quests)
            .WithOne(q => q.Grade)
            .HasForeignKey(q => q.GradeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}