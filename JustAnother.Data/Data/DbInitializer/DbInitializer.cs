using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JustAnother.DataAccess.Data.DbInitializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _db;

    private readonly RoleManager<IdentityRole> _roleManager;
    public DbInitializer(ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    public async void Initialize()
    {
        if (_db.Database.GetPendingMigrations().Any())
        {
            _db.Database.Migrate();
        }

        if (!_roleManager.RoleExistsAsync(StaticDetails.Role_Admin).GetAwaiter().GetResult())
        {
             _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).Wait();
             _roleManager.CreateAsync(new IdentityRole(StaticDetails.Role_User)).Wait();
        }
    }
}
