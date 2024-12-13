using JustAnother.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JustAnother.DataAccess.Data.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _db;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<ApplicationUser> _userManager;
    public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InitializeAsync()
    {
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }

        if (!await _roleManager.RoleExistsAsync(StaticDetails.Role_Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin));
            await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User));

            ApplicationUser user = new()
            {
                Name = "Main Admin",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "1234567890",
            };

            var result = await _userManager.CreateAsync(user, "admin123");

            if (result.Succeeded)
            {
                ApplicationUser userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(l => l.UserName == "admin@gmail.com");
                await _userManager.AddToRoleAsync(userFromDb, StaticDetails.Role_Admin);
            }
        }
    }
}
