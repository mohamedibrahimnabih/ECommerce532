using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public IActionResult Index(string? query, int page = 1, int size = 2)
    {
        var categories = _db.Categories.AsQueryable();

        if (query is not null)
            categories = categories.Where(e => e.Name.ToLower().Contains(query.ToLower()));

        // ViewBag Vs ViewData (MVC only)
        //ViewBag.Query = query ?? "";
        //ViewData["Query"] = query ?? "";

        var totalPages = Math.Ceiling(categories.Count() / (double)size);
        categories = categories.Skip((page - 1) * size).Take(size);

        return View(new CategoryWithFilterVM
        {
            Categories = categories,
            Query = query ?? "",
            TotalPages = totalPages,
            CurrentPage = page,
        });
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Update()
    {
        return View();
    }
}
