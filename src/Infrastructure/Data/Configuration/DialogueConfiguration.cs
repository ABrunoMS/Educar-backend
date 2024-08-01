using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class DialogueConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<Dialogue>
{
    private readonly DatabaseFacade _database = database;

    public void Configure(EntityTypeBuilder<Dialogue> builder)
    {
        builder.Property(t => t.Text).IsRequired();
        builder.Property(t => t.Order).IsRequired();
        builder.Property(t => t.NpcId).IsRequired();
    }
}