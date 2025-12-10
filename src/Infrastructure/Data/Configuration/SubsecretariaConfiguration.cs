using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // Adicione este using
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

// 1. ADICIONE O CONSTRUTOR PRIMÁRIO AQUI
public class SubsecretariaConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Subsecretaria>
{
    // 2. ADICIONE O CAMPO PRIVADO
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Subsecretaria> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(x => x.Client)
            .WithMany(c => c.Subsecretarias)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Regionais)
            .WithOne(x => x.Subsecretaria)
            .HasForeignKey(x => x.SubsecretariaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
