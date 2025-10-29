using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    // O construtor (DatabaseFacade) pode ser removido se não for usado
    // para lógica condicional (como checar o tipo de banco).
    
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property(t => t.ContractDurationInYears).IsRequired();
        builder.Property(t => t.ContractSigningDate).IsRequired();
        builder.Property(t => t.ImplementationDate).IsRequired();
        builder.Property(t => t.TotalAccounts).IsRequired();
        builder.Property(t => t.Status).IsRequired().HasConversion<string>();

        builder.HasOne(c => c.Client)
            .WithMany() 
            .HasForeignKey(c => c.ClientId)
            .IsRequired(false); 

        builder.HasMany(c => c.ContractProducts)
            .WithOne(cp => cp.Contract)
            .HasForeignKey(cp => cp.ContractId);;

        builder.HasMany(c => c.ContractContents)
            .WithOne(cc => cc.Contract)
            .HasForeignKey(cc => cc.ContractId);
    }
}