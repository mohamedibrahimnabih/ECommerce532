using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ECommerce532.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    // Service layer => UserStore<ApplicationUser>
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;

    public AccountController(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM registerVM)
    {
        if (!ModelState.IsValid)
            return View(registerVM);

        //ApplicationUser user = new()
        //{
        //    FirstName = registerVM.FirstName,
        //    LastName = registerVM.LastName,
        //    Email = registerVM.Email,
        //    UserName = registerVM.UserName,
        //    PasswordHash = registerVM.Password,
        //    Address = registerVM.Address,
        //};

        //TypeAdapterConfig config = new();
        //config.NewConfig<RegisterVM, ApplicationUser>()
        //    .Map("FirstName", "FName")
        //    .Map("LastName", "LName");

        ApplicationUser user = registerVM.Adapt<ApplicationUser>(/*config*/);

        var result = await _userManager.CreateAsync(user, registerVM.Password);

        if(!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(registerVM);
        }

        {
            // Send confirmation mail
            // generate unique token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), ControllerConstants.ACCOUNT_CONTROLLER, new { area = AreaConstants.IDENTITY_AREA, user.Id, token }, Request.Scheme);
            string body = $"<h1>Please confirm your account by clicking <b><a href='{link}'>here</a></b></h1>";

            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Account", body);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Create Account Successfully, please verify your account";

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> Confirm(string id, string token)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            TempData[NotificationConstants.ERROR_NOTIFICATION] = String.Join(", ", result.Errors.Select(e => e.Description));
        else
        {
            TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Confirm Account successfully, please login";
            //await _signInManager.SignInAsync(user, false);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM)
    {
        if (!ModelState.IsValid)
            return View(loginVM);

        // 1. Check user name or email 
        var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ??
                                await _userManager.FindByNameAsync(loginVM.EmailOrUserName);

        if (user is null)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
            ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            return View(loginVM);
        }

        #region Old way
        //// 2. Check password
        //bool result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

        //if (!result)
        //{
        //    ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
        //    ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

        //    return View(loginVM);
        //}

        //// 3. Login & Check remember me
        //await _signInManager.SignInAsync(user, loginVM.Remember); 
        #endregion

        var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.Remember, lockoutOnFailure: true);

        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Too many attempts, please try again later");
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
            ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            return View(loginVM);
        }

        if(signInResult.IsNotAllowed)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Please verify your account!");
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Welcome Back {user.FirstName} {user.LastName}";

        return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });
    }

    /***
     * TODO
     */
    [HttpGet]
    public IActionResult ResendConfirmation()
    {
        // generate view

        return View();
    }

    [HttpPost]
    public IActionResult ResendConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
    {
        // 1. generate new token
        // 2. generate link
        // 3. generate new body
        // 4. send email
        // 5. redirect to login

        return View();
    }

    public IActionResult ExternalLogin()
    {
        return View();
    }

    public IActionResult Logout()
    {
        return View();
    }

    public IActionResult ForgetPassword()
    {
        return View();
    }
}
