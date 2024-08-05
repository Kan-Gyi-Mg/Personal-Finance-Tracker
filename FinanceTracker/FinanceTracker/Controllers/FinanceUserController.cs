using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.FinanceUserView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Controllers
{
    public class FinanceUserController : Controller
    {
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _usermanager;
        private readonly FinanceDbContext _context;
        public FinanceUserController(SignInManager<FinanceUser> signInManager,UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _usermanager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        //show all user
        [HttpGet]
        public async Task<IActionResult> ShowUserList()
        {
            var User = _usermanager.Users;
            return View(User);
        }
        //create user
        [HttpGet]
        public async Task<IActionResult> UserCreate() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new FinanceUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };
            var result = await _usermanager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                var roleresult = await _usermanager.AddToRoleAsync(user, "User");
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to assign role.");
                    return View(model);
                }

                return RedirectToAction("UserLogin","FinanceUser");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        //login user
        [HttpGet]
        public async Task<IActionResult> UserLogin() => View();
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            FinanceUser user = null;

            if (model.EmailOrUserName.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                user = await _usermanager.FindByEmailAsync(model.EmailOrUserName);
            }
            else
            {
                user = await _usermanager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var roles = await _usermanager.GetRolesAsync(user);
                    Console.WriteLine("++++++++++++++++++++" + string.Join(", ", roles));

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "FinanceAdmin");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("StartingPage", "Operation");
                    }
                    else if (roles.Contains("PremiumUser"))
                    {
                        return RedirectToAction("StartingPage", "Operation");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        //logout user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("UserLogin", "FinanceUser");
        }
    }
}
