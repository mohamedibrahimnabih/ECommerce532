using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
public class CategoryController : Controller
{
    //private readonly ApplicationDbContext _db = new();
    private readonly IRepository<Category> _repository;// = new Repository<Category>();

    public CategoryController(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? query, int page = 1, int size = 4)
    {
        var categories = _repository.Get();

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
        return View(new Category());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(category);

        //_db.Categories.Add(new Category()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        //_db.Categories.Add(category);
        //_db.SaveChanges();

        await _repository.CreateAsync(category, ct);
        await _repository.CommitAsync(ct);

        //Response.Cookies.Append(NotificationConstants.SUCCESS_NOTIFICATION, "Create Category Successfully");
        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Create Category Successfully";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        //var category = _db.Categories.AsNoTracking().FirstOrDefault(e => e.Id == id);
        var category = _repository.GetOne(e => e.Id == id, tracked: false);

        if (category is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Category category, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(category);

        //_db.Categories.Add(new Category()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        _repository.Update(category);
        await _repository.CommitAsync(ct);

        //Response.Cookies.Append(NotificationConstants.SUCCESS_NOTIFICATION, "Update Category Successfully");
        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Update Category Successfully";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var category = _repository.GetOne(e => e.Id == id);

        if (category is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        _repository.Delete(category);
        await _repository.CommitAsync(ct);

        //Response.Cookies.Append(NotificationConstants.SUCCESS_NOTIFICATION, "Delete Category Successfully");
        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Delete Category Successfully";

        return RedirectToAction(nameof(Index));
    }
}
