using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContractProductConfiguration : IEntityTypeConfiguration<ContractProduct>
{
    public void Configure(EntityTypeBuilder<ContractProduct> builder)
    {
        builder.HasKey(cp => new { cp.ContractId, cp.ProductId });

        builder
            .HasOne(cp => cp.Contract)
            .WithMany(c => c.ContractProducts)
            .HasForeignKey(cp => cp.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(cp => cp.Product)
            .WithMany(p => p.ContractProducts)
            .HasForeignKey(cp => cp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
