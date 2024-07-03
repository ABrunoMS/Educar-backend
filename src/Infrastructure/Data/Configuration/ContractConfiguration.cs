using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property(t => t.ContractDurationInYears).IsRequired();
        builder.Property(t => t.ContractSigningDate).IsRequired();
        builder.Property(t => t.ImplementationDate).IsRequired();
        builder.Property(t => t.TotalAccounts).IsRequired();
        builder.Property(t => t.Status).IsRequired().HasConversion<string>();
    }
}