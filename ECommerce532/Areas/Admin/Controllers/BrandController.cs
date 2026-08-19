using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Admin.Controllers;

public class BrandController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
