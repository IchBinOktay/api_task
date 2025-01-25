using Common.Constants;
using Common.Entities;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
           await AddRolesAsync(roleManager);
           await AddAdminAsync(userManager, roleManager);
        }
        private static async Task AddRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues<UserRoles>())
            {
                if (!await roleManager.RoleExistsAsync(role.ToString()))
                {
                    _ = roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()
                    }).Result;
                }
            }
        }
        private static async Task AddAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (await userManager.FindByEmailAsync("admin@app.com") is null)
            {

                var user = new User
                {
                    UserName = "admin@app.com",
                    Email = "admin@app.com",
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (!result.Succeeded)
                    throw new Exception("Could not add admin");

                var role = await roleManager.FindByNameAsync("Admin");
                if (role?.Name is null)
                    throw new Exception("Could not find admin role");

                var addToResult = await userManager.AddToRoleAsync(user, role.Name);
                if (!addToResult.Succeeded)
                    throw new Exception("It was not possible to add the admin role to the user");
            }
        }
    }
}
