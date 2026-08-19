using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Admin.Controllers;

public class ProductController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
