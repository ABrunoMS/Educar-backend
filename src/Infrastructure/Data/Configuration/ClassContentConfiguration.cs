using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ClassContentConfiguration : IEntityTypeConfiguration<ClassContent>
{
    public void Configure(EntityTypeBuilder<ClassContent> builder)
    {
        builder.HasKey(cc => new { cc.ClassId, cc.ContentId });
    }
}