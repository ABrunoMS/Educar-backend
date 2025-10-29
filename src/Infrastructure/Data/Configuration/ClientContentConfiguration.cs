using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClientContentConfiguration : IEntityTypeConfiguration<ClientContent>
{
    public void Configure(EntityTypeBuilder<ClientContent> builder)
    {
        builder.HasKey(cc => new { cc.ClientId, cc.ContentId });
    }
}