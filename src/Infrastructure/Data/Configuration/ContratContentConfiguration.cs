using Educar.Backend.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContractContentConfiguration : IEntityTypeConfiguration<ContractContent>
{
    public void Configure(EntityTypeBuilder<ContractContent> builder)
    {
        builder.HasKey(cc => new { cc.ContractId, cc.ContentId });
    }
}