using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class AccountConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Account>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Email).IsUnique();
        builder.Property(t => t.RegistrationNumber);
        builder.Property(t => t.AverageScore).HasColumnType("decimal(5,2)");
        builder.Property(t => t.EventAverageScore).HasColumnType("decimal(5,2)");
        builder.Property(t => t.Stars);
        builder.Property(t => t.ClientId).IsRequired();
        builder.Property(t => t.Role).IsRequired().HasConversion<string>();

        builder
            .HasMany(a => a.AccountClasses)
            .WithOne(ac => ac.Account)
            .HasForeignKey(ac => ac.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(a => a.Answers)
            .WithOne(ans => ans.Account)
            .HasForeignKey(ans => ans.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}