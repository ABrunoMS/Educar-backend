using Educar.Backend.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ProductContentConfiguration : IEntityTypeConfiguration<ProductContent>
{
    public void Configure(EntityTypeBuilder<ProductContent> builder)
    {
        builder.HasKey(pc => new { pc.ProductId, pc.ContentId });
    }
}