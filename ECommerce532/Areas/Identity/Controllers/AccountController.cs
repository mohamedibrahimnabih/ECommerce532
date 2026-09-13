using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ECommerce532.Areas.Identity.Controllers;

[Area(AreaConstants.IDENTITY_AREA)]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager; // Service layer => UserStore<ApplicationUser>
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

    public AccountController(UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IEmailSender emailSender,
        IRepository<ApplicationUserOTP> applicationUserOTPRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _applicationUserOTPRepository = applicationUserOTPRepository;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if(User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });

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

        await _userManager.AddToRoleAsync(user, RoleConstants.CUSTOMER);

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
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });

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

        if (signInResult.IsNotAllowed)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Please verify your account!");
        }

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");
            ModelState.AddModelError(nameof(LoginVM.Password), "Invalid Password");

            return View(loginVM);
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
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });

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

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = "Logout successfully";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return View(forgetPasswordVM);

        var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ??
                                await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);

        if (user is null)
        {
            ModelState.AddModelError(nameof(LoginVM.EmailOrUserName), "Invalid User Name or Email");

            return View(forgetPasswordVM);
        }

        string otp = new Random().Next(1000, 9999).ToString();

        await _applicationUserOTPRepository.CreateAsync(new()
        {
            ApplicationUserId = user.Id,
            OTP = otp,
        }, ct);
        await _applicationUserOTPRepository.CommitAsync(ct);


        string body = $"<h1>Your otp number is: {otp}. don't share it.</h1>";
        await _emailSender.SendEmailAsync(user.Email!, "Reset your account", body);

        TempData["RedirectToValidateOTP"] = Guid.NewGuid();
        Response.Cookies.Append("userId", user.Id);

        return RedirectToAction(nameof(ValidateOTP));
    }

    [HttpGet]
    public IActionResult ValidateOTP()
    {
        if (User.Identity is not null && User.Identity.IsAuthenticated)
            return RedirectToAction(nameof(Index), ControllerConstants.HOME_CONTROLLER, new { area = AreaConstants.CUSTOMER_AREA });

        if (TempData["RedirectToValidateOTP"] is null)
            return NotFound();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTP)
    {
        if (!ModelState.IsValid)
            return View(validateOTP);

        var userId = Request.Cookies["userId"];
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var otpInDB = _applicationUserOTPRepository
            .Get(e => e.ApplicationUserId == userId && !e.IsUsed && e.ValidTo >= DateTime.UtcNow)
            .OrderBy(e => e.CreateAt)
            .LastOrDefault();

        if (otpInDB is null || otpInDB.OTP is null || validateOTP.OTP != otpInDB.OTP)
        {
            TempData[NotificationConstants.ERROR_NOTIFICATION] = $"Invalid OTP";

            TempData["RedirectToValidateOTP"] = Guid.NewGuid();
            return RedirectToAction(nameof(ValidateOTP));
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Valid OTP, you can now change your password";
        otpInDB.IsUsed = true;
        await _applicationUserOTPRepository.CommitAsync();

        return RedirectToAction(nameof(NewPassword));
    }

    [HttpGet]
    public IActionResult NewPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
    {
        if (!ModelState.IsValid)
            return View(newPasswordVM);

        var userId = Request.Cookies["userId"];
        if (userId is null) return NotFound();

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

        if (!result.Succeeded)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }

            return View(newPasswordVM);
        }

        TempData[NotificationConstants.SUCCESS_NOTIFICATION] = $"Reset Password successfully";
        Response.Cookies.Delete("userId");

        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
