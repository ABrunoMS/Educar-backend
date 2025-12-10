using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

// O construtor que o seu sistema espera
public class RegionalConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Regional>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Regional> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(r => r.Subsecretaria)
            .WithMany(s => s.Regionais)
            .HasForeignKey(r => r.SubsecretariaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Schools)
            .WithOne(s => s.Regional)
            .HasForeignKey(s => s.RegionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
