using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce532.DataAccess.EntityConfigurations;

public class ProductSubImgEntityTypeConfiguration : IEntityTypeConfiguration<ProductSubImg>
{
    public void Configure(EntityTypeBuilder<ProductSubImg> builder)
    {
        builder.Property(e => e.SubImg).IsRequired().HasMaxLength(500);
    }
}
