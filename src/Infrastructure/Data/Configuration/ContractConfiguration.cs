using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContractConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Contract>
{
    private readonly DatabaseFacade _database = database;
    
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property(t => t.ContractDurationInYears).IsRequired();
        builder.Property(t => t.ContractSigningDate).IsRequired();
        builder.Property(t => t.ImplementationDate).IsRequired();
        builder.Property(t => t.TotalAccounts).IsRequired();
        builder.Property(t => t.Status).IsRequired().HasConversion<string>();
        builder.Property(t => t.ClientId).IsRequired();
        builder.Property(t => t.GameId).IsRequired();
    }
}