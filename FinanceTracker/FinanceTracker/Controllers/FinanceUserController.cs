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
        public async Task<IActionResult> UserCreate(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
        //login user
        [HttpGet]
        public async Task<IActionResult> UserLogin()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> UserLogin(LoginViewModel model)
        {
            return View(model);
        }

        //logout user
        [HttpPost]
        public async Task<IActionResult> UserLogout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("UserLogin", "FinanceUser");
        }
    }
}
