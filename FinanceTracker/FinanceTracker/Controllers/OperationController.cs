using FinanceTracker.DbClass;
using FinanceTracker.Models.News;
using FinanceTracker.Models.Operations;
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
        [HttpGet]
        public async Task<IActionResult> CashCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CashCreate(UserCashModel model)
        {
            FinanceUser user = await _userManager.GetUserAsync(User);
            var addnews = new UserCashModel
            {
                UserId = user.Id,
                Category = model.Category,
                Amount = model.Amount,
            };
            await _context.usercash.AddAsync(addnews);
            await _context.SaveChangesAsync();
            return RedirectToAction("CalculateDashboard", "Operation");
        }
        [HttpGet]
        public async Task<IActionResult> CashEdit(int cashid)
        {
            var cash = await _context.usercash.FirstOrDefaultAsync(n => n.cashId == cashid);
            return View(cash);
        }
        [HttpPost]
        public async Task<IActionResult> CashEdit(UserCashModel model)
        {
            var editcash = await _context.usercash.FirstOrDefaultAsync(n => n.cashId == model.cashId);
            editcash.Category = model.Category;
            editcash.Amount = model.Amount;
            _context.usercash.Update(editcash);
            await _context.SaveChangesAsync();
            return RedirectToAction("CalculateDashboard", "Operation");
        }
        [HttpPost]
        public async Task<IActionResult> CashDelete(int cashid)
        {
            var newlist = await _context.usercash.FirstOrDefaultAsync(n => n.cashId == cashid);
            _context.usercash.Remove(newlist);
            await _context.SaveChangesAsync();
            return RedirectToAction("CalculateDashboard", "Operation");
        }
    }
}
