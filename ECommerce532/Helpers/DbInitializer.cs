using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce532.Helpers;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public DbInitializer(ApplicationDbContext context, 
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public void Initialize()
    {
        // migrate db
        if (_context.Database.GetPendingMigrations().Any())
            _context.Database.Migrate();

        // seed roles
        if(_roleManager.Roles.IsNullOrEmpty())
        {
            _roleManager.CreateAsync(new(RoleConstants.SUPER_ADMIN)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new(RoleConstants.ADMIN)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new(RoleConstants.EMPLOYEE)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new(RoleConstants.CUSTOMER)).GetAwaiter().GetResult();


            // seed super account user
            ApplicationUser user = new()
            {
                UserName = "SuperAdmin",
                FirstName = "Super",
                LastName = "Admin",
                Email = "SuperAdmin@eraasoft.com",
                EmailConfirmed = true
            };

            _userManager.CreateAsync(user, "Admin123#").GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(user, RoleConstants.SUPER_ADMIN).GetAwaiter().GetResult();
        }
    }
}
