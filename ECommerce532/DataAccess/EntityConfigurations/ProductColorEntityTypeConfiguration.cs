using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce532.DataAccess.EntityConfigurations;

public class ProductColorEntityTypeConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.Property(e => e.Color).IsRequired().HasMaxLength(9);
    }
}
