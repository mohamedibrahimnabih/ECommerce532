using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public IActionResult Index(string? query, int page = 1, int size = 4)
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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        //_db.Categories.Add(new Category()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        _db.Categories.Add(category);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var category = _db.Categories.AsNoTracking().FirstOrDefault(e => e.Id == id);

        if (category is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        return View(category);
    }

    [HttpPost]
    public IActionResult Update(Category category)
    {
        //_db.Categories.Add(new Category()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        _db.Categories.Update(category);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var category = _db.Categories.FirstOrDefault(e => e.Id == id);

        if (category is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        _db.Categories.Remove(category);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
