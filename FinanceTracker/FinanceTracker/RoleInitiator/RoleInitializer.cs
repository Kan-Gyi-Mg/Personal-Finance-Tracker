using FinanceTracker.Models.User;
using Microsoft.AspNetCore.Identity;

namespace FinanceTracker.RoleInitiator
{
    public class RoleInitializer : IRoleInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<FinanceUser> _userManager;

        public RoleInitializer(RoleManager<IdentityRole> roleManager, UserManager<FinanceUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            string[] roleNames = { "Admin", "User", "PremiumUser" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create a default admin user if necessary
            string adminUserName = "admin";
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@12345";

            if (await _userManager.FindByNameAsync(adminUserName) == null)
            {
                FinanceUser adminUser = new FinanceUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
