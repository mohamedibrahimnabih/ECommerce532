using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce532.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ApplicationUser applicationUser)
    {
        //var result = await _userManager.UpdateAsync(applicationUser);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        user.FirstName = applicationUser.FirstName;
        user.LastName = applicationUser.LastName;
        user.Email = applicationUser.Email;
        user.PhoneNumber = applicationUser.PhoneNumber;
        user.Address = applicationUser.Address;

        var result = await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM changePasswordVM)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPassword, changePasswordVM.NewPassword);

        if(!result.Succeeded)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = String.Join(", ", result.Errors.Select(e => e.Description));
            return View(nameof(Index), user);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Change password successfully";
        return RedirectToAction(nameof(Index));
    }
}
