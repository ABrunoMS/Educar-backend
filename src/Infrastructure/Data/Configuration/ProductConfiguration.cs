using Educar.Backend.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasMany(p => p.ProductContents)
               .WithOne(pc => pc.Product)
               .HasForeignKey(pc => pc.ProductId);

        builder.HasMany(p => p.ClientProducts)
               .WithOne(cp => cp.Product)
               .HasForeignKey(cp => cp.ProductId);
               
        builder.HasMany(p => p.ContractProducts)
           .WithOne(cp => cp.Product)
           .HasForeignKey(cp => cp.ProductId);
       
       builder.HasMany(p => p.ClassProducts)
              .WithOne(cp => cp.Product)
              .HasForeignKey(cp => cp.ProductId);
     
    }
}