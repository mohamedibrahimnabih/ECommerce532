using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerce532;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
        builder.Services.AddScoped<IRepository<Brand>, Repository<Brand>>();
        builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
        builder.Services.AddScoped<IBulkRepository<ProductSubImg>, BulkRepository<ProductSubImg>>();
        builder.Services.AddScoped<IBulkRepository<ProductColor>, BulkRepository<ProductColor>>();

        var connectionString =
                        builder.Configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string"
                            + "'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(connectionString);
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
