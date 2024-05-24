using Microsoft.AspNetCore.Identity;

public interface ISeedUserRoleInitial
{
    Task SeedRolesAsync();
    Task SeedUserAsync();
}

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SeedUserRoleInitial(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
    }

    public async Task SeedUserAsync()
    {
        var user = new IdentityUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            EmailConfirmed = true
        };

        var userExists = await _userManager.FindByEmailAsync(user.Email);
        if (userExists == null)
        {
            var result = await _userManager.CreateAsync(user, "Password@123");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
