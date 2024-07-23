using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class AccountClassConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<AccountClass>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<AccountClass> builder)
    {
        builder.HasKey(ac => new { ac.AccountId, ac.ClassId });

        builder.HasOne(ac => ac.Account)
            .WithMany(a => a.AccountClasses)
            .HasForeignKey(ac => ac.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ac => ac.Class)
            .WithMany(c => c.AccountClasses)
            .HasForeignKey(ac => ac.ClassId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}