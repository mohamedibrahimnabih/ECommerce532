using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Admin.Controllers;

[Area(AreaConstants.ADMIN_AREA)]
[Authorize(Roles = $"{RoleConstants.SUPER_ADMIN}")]
public class UserController : Controller
{
    // 1. get all user from userManager.Users
    // 2. filter
    // 3. pagination
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ChangeRole(string userId, string newRole)
    {
        return View();
    }
}
