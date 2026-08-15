using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce532.DataAccess.EntityConfigurations;

public class BrandEntityTypeConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Logo).IsRequired().HasMaxLength(500);
    }
}
