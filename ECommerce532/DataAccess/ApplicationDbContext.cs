using ECommerce532.DataAccess.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductSubImg> ProductSubImgs { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BrandEntityTypeConfiguration).Assembly);
    }

    #region Legacy Code
    //public ApplicationDbContext()
    //{
    //}

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    base.OnConfiguring(optionsBuilder);

    //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ECommerce532;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");
    //}
    #endregion
}
