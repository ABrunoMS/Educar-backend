using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClientProductConfiguration : IEntityTypeConfiguration<ClientProduct>
{
    public void Configure(EntityTypeBuilder<ClientProduct> builder)
    {
        builder.HasKey(cp => new { cp.ClientId, cp.ProductId });
    }
}