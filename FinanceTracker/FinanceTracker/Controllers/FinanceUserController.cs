using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.FinanceUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Controllers
{
    public class FinanceUserController : Controller
    {
        private readonly UserManager<FinanceUser> _usermanager;
        private readonly FinanceDbContext _context;
        public FinanceUserController(UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _usermanager = userManager;
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> ShowUserList()
        {
            var User = _usermanager.Users;
            return View(User);
        }
        [HttpGet]
        public async Task<IActionResult> UserCreate() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
