using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.operationView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        public AdminController(EmailService emailService, SignInManager<FinanceUser> signInManager, UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var users = _userManager.Users.ToList();
            var userRoleViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                // Get the user's roles (assuming each user has exactly one role)
                var roles = await _userManager.GetRolesAsync(user);
                var userRoleViewModel = new AdminUserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault()
                };
                userRoleViewModels.Add(userRoleViewModel);
            }

            return View(userRoleViewModels);
        }
        [HttpGet]
        public async Task<IActionResult> UserEdit(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var model = new AdminUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(AdminUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            user.UserName = model.UserName;
            user.Email = model.Email;
            var result = await _userManager.UpdateAsync(user);
            return RedirectToAction("Dashboard","Admin");
        }
        [HttpPost]
        public async Task<IActionResult> UserDelete(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Dashboard","Admin"); 
            }
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
