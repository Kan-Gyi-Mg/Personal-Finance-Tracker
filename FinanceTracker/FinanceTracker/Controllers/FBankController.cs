using FinanceTracker.DbClass;
using FinanceTracker.Models.Operations;
using FinanceTracker.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    [Authorize(Roles = "PremiumUser")]
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
        public async Task<IActionResult> ShowBankStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            var bank = _context.fbank.FirstOrDefault(i => i.UserId == user.Id);
            return View(bank);
        }
        [HttpPost]
        public async Task<IActionResult> BankInput(FBankModel model)
        {
            
            var fbank = _context.fbank.FirstOrDefault(i => i.FBankid == model.FBankid);
            fbank.BankAmount = fbank.BankAmount + model.BankAmount;
            _context.fbank.Update(fbank);
            _context.SaveChanges();
            return RedirectToAction("ShowBankStatus","FBank");

        }
        [HttpPost]
        public async Task<IActionResult> BankOutput(FBankModel model)
        {
            
            var fbank = _context.fbank.FirstOrDefault(i => i.FBankid == model.FBankid);
            fbank.BankAmount = fbank.BankAmount - model.BankAmount;
            _context.fbank.Update(fbank);
            _context.SaveChanges();
            return RedirectToAction("ShowBankStatus", "FBank");

        }
    }
}
