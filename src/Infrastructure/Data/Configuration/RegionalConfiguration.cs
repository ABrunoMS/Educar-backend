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
        // Define a chave primária
        builder.HasKey(r => r.Id);

        // Define que a propriedade Nome é obrigatória e tem tamanho máximo
        // (Use 'Name' em vez de 'Nome' para seguir a convenção do C#)
        builder.Property(r => r.Nome)
            .HasMaxLength(100)
            .IsRequired();

        // Define a relação com Subsecretaria
        // Isso assume que sua entidade 'Subsecretaria' tem uma lista 'public List<Regional> Regionais'
        builder.HasOne(r => r.Subsecretaria)
            .WithMany(s => s.Regionais)
            .HasForeignKey(r => r.SubsecretariaId)
            .OnDelete(DeleteBehavior.Cascade); // <-- Ponto de Atenção
    }
}