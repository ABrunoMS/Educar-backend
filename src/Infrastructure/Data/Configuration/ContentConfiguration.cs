using Educar.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Educar.Backend.Infrastructure.Data.Configuration;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.HasMany(c => c.ProductContents)
               .WithOne(pc => pc.Content)
               .HasForeignKey(pc => pc.ContentId);

        builder.HasMany(c => c.ContractContents)
               .WithOne(cc => cc.Content)
               .HasForeignKey(cc => cc.ContentId);

        builder.HasMany(c => c.ClientContents)
               .WithOne(cc => cc.Content)
               .HasForeignKey(cc => cc.ContentId);
        
       builder.HasMany(c => c.ClassContents)
              .WithOne(cc => cc.Content)
              .HasForeignKey(cc => cc.ContentId);
              
    }
}