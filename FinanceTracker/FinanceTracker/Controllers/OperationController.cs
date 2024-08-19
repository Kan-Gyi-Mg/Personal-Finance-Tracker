using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.operationView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Controllers
{
    public class OperationController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        public OperationController(EmailService emailService, SignInManager<FinanceUser> signInManager, UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        //landing page of the site
        public IActionResult StartingPage()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CalculateDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var item = await _context.usercash.Where(n => n.UserId == user.Id).ToListAsync();
            int total = 0;
            foreach(var i in item)
            {
                total = total + i.Amount;
            }
            var newmodel = new UserCashViewModel
            {
                usercash = item,
                total = total,
            };
            return View(newmodel);
        }
    }
}
