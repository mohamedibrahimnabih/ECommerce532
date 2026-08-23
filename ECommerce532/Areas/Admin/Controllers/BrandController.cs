using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
public class BrandController : Controller
{
    private readonly ApplicationDbContext _db = new();

    public IActionResult Index(string? query, int page = 1, int size = 4)
    {
        var brands = _db.Brands.AsQueryable();

        if (query is not null)
            brands = brands.Where(e => e.Name.ToLower().Contains(query.ToLower()));

        // ViewBag Vs ViewData (MVC only)
        //ViewBag.Query = query ?? "";
        //ViewData["Query"] = query ?? "";

        var totalPages = Math.Ceiling(brands.Count() / (double)size);
        brands = brands.Skip((page - 1) * size).Take(size);

        return View(new BrandWithFilterVM
        {
            Brands = brands,
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
    public IActionResult Create(Brand brand, IFormFile Img) // photo.png
    {
        if (Img is not null && Img.Length > 0)
        {
            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
            //var fileName = Img.FileName + DateTime.Now.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName); 
            var fileName = $"{Guid.NewGuid().ToString()}-{DateTime.Now.ToString("dd-MM-yyyy")}{Path.GetExtension(Img.FileName)}"; 

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "brands", fileName);

            using(var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }

            brand.Logo = fileName;
        }

        //_db.Brands.Add(new Brand()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});

        _db.Brands.Add(brand);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Update(int id)
    {
        var brand = _db.Brands.AsNoTracking().FirstOrDefault(e => e.Id == id);

        if (brand is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        return View(brand);
    }

    [HttpPost]
    public IActionResult Update(Brand brand, IFormFile Img)
    {
        var brandInDB = _db.Brands.AsNoTracking().FirstOrDefault(e => e.Id == brand.Id);

        if (brandInDB is null) return NotFound();

        if (Img is not null && Img.Length > 0)
        {
            // Save New Img in wwwroot

            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Img.FileName);
            //var fileName = Img.FileName + DateTime.Now.ToString("dd-MM-yyyy") + Path.GetExtension(Img.FileName); 
            var fileName = $"{Guid.NewGuid().ToString()}-{DateTime.Now.ToString("dd-MM-yyyy")}{Path.GetExtension(Img.FileName)}";

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "brands", fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stream);
            }

            // Delete Old Img from wwwroot

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", "brands", brandInDB.Logo);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            // Replace img in DB

            brand.Logo = fileName;
        }
        else
            brand.Logo = brandInDB.Logo;

        //_db.Brands.Add(new Brand()
        //{
        //    Name = name,
        //    Description = Description,
        //    Status = status
        //});
        _db.Brands.Update(brand);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var brand = _db.Brands.FirstOrDefault(e => e.Id == id);

        if (brand is null)
            return RedirectToAction(nameof(HomeController.NotFoundPage), ControllerConstants.HOME_CONTROLLER);

        _db.Brands.Remove(brand);
        _db.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
