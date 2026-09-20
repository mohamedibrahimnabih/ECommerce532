using ECommerce532.DataAccess.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
    public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

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
