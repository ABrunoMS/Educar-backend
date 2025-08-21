using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configurations;

public class AccountSchoolConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<AccountSchool>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<AccountSchool> builder)
    {
        builder.HasKey(asc => new { asc.AccountId, asc.SchoolId });

        builder.HasOne(asc => asc.Account)
            .WithMany(a => a.AccountSchools)
            .HasForeignKey(asc => asc.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(asc => asc.School)
            .WithMany(s => s.AccountSchools)
            .HasForeignKey(asc => asc.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}