using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClassProductConfiguration : IEntityTypeConfiguration<ClassProduct>
{
    public void Configure(EntityTypeBuilder<ClassProduct> builder)
    {
        builder.HasKey(cp => new { cp.ClassId, cp.ProductId });
    }
}