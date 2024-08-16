using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Metadata;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;


    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;

    }

    public async Task SeedSuperUser()
    {
        var roleStore = new RoleStore<IdentityRole>(_context);
        var userStore = new UserStore<ApplicationUser>(_context);

        var newUser = new ApplicationUser
        {
            UserName = "SuperUser",
            NormalizedUserName = "superuser",
            Email = "superuser@example.com",
            NormalizedEmail = "superuser@example.com",
            EmailConfirmed = false,
            LockoutEnabled = true,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var hasher = new PasswordHasher<ApplicationUser>();
        var hashedPassword = hasher.HashPassword(newUser, "superadmin");
        newUser.PasswordHash = hashedPassword;

        var hasAdminRole = await roleStore.Roles.AnyAsync(role => role.Name == "Admin");
        if (!hasAdminRole)
        {
            await roleStore.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "admin" });
        }

        var hasSuperUser = await userStore.Users.AnyAsync(u => u.NormalizedUserName == newUser.UserName);
        if (!hasSuperUser)
        {
            await userStore.CreateAsync(newUser);
            await userStore.AddToRoleAsync(newUser, "Admin"); 
        }

        await _context.SaveChangesAsync();
    }


}
