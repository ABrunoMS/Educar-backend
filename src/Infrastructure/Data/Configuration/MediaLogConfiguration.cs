using Educar.Backend.Application.Common.Extensions;
using Educar.Backend.Application.Common.Interfaces;
using Educar.Backend.Domain.Entities;
using Educar.Backend.Infrastructure.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class MediaLogConfiguration(DatabaseFacade database) : IEntityTypeConfiguration<MediaLog>
{
    
    public void Configure(EntityTypeBuilder<MediaLog> builder)
    {

        builder.Property(t => t.MediaId).IsRequired();
        builder.Property(t => t.AccountId).IsRequired();
        builder.ConfigureJsonProperty(nameof(MediaLog.CurrentState), database).IsRequired();
        builder.ConfigureJsonProperty(nameof(MediaLog.PreviousState), database);
        builder.Property(t => t.Action).IsRequired().HasConversion<string>();
    }
}