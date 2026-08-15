using ECommerce532.Enums;
using ECommerce532.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce532.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public IActionResult Index(ProductFilter filter)
    {
        var products = _db.Products.AsQueryable();

        products = products.Include(e => e.Category);
        
        switch(filter)
        {
            case ProductFilter.AllProducts:
                break;
            case ProductFilter.TopView:
                products = products.OrderByDescending(e => e.Traffic);
                break;
            case ProductFilter.TopDiscount:
                products = products.OrderByDescending(e => e.Discount);
                break;
        }

        products = products.Skip(0).Take(8);

        return View(products.AsEnumerable());
    }

    public IActionResult Details([FromRoute] int id)
    {
        var product = _db.Products
            .AsNoTracking()
            .Include(e => e.Category)
            .SingleOrDefault(e => e.Id == id);

        if (product is null)
            return RedirectToAction(nameof(NotFoundPage));

        var relatedProduct = _db.Products
            .Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id)
            .Skip(0)
            .Take(4);

        // TODO 1
        // TODO 2

        return View(new ProductWithRelatedVM
        {
            Product = product,
            RelatedProducts = relatedProduct
        });
    }

    public IActionResult NotFoundPage()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Welcome()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
