using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    public class FBankController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        public FBankController(EmailService emailService, SignInManager<FinanceUser> signInManager, UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        public IActionResult ShowBankStatus()
        {
            return View();
        }
    }
}
