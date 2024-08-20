using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.FinanceUserView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanceTracker.RoleInitiator;
using FinanceTracker.Models.Operations;

namespace FinanceTracker.Controllers
{
    public class FinanceUserController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public FinanceUserController(RoleManager<IdentityRole> roleManager,EmailService emailService,SignInManager<FinanceUser> signInManager,UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                var roleresult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to assign role.");
                    return View(model);
                }
                var otp = GenerateOTP();
                HttpContext.Session.SetString("OTP", otp);
                HttpContext.Session.SetString("RegisterUserId", user.Id);

                await _emailService.SendEmailAsync(model.Email, "OTP Code", $"Your OTP code is {otp}");
                return RedirectToAction("VerifyOTP");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        //forgot password
        [HttpGet]
        public async Task<IActionResult> UserForgotPassword() => View();
        [HttpPost]
        public async Task<IActionResult> UserForgotPassword(String Email)
        {
            if(Email == null)
            {
                return View(Email);
            }
            var user = await _userManager.FindByEmailAsync(Email);
            user.ForgotPassword = true;
            var otp = GenerateOTP();
            HttpContext.Session.SetString("ForgotOTP", otp);
            HttpContext.Session.SetString("ForgotUserId", user.Id);
            await _emailService.SendEmailAsync(Email, "OTP Code", $"Your OTP code is {otp}");
            return RedirectToAction("VerifyForgotOTP");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePremium(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (!await _roleManager.RoleExistsAsync("PremiumUser"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("PremiumUser"));
                if (!roleResult.Succeeded)
                {
                    return BadRequest("Failed to create role.");
                }
            }
            if (!await _userManager.IsInRoleAsync(user, "PremiumUser"))
            {
                var result1 = await _userManager.RemoveFromRoleAsync(user, "User");
                var result = await _userManager.AddToRoleAsync(user, "PremiumUser");
                if (!result.Succeeded)
                {
                    return NotFound();
                }   
            }
            var fbank = new FBankModel
            {
                UserId = user.Id,
                BankAmount = 0
            };
            await _context.fbank.AddAsync(fbank);
            _context.SaveChanges();
            return RedirectToAction("StartingPage", "Operation");
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
                user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
            }
            else
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine("++++++++++++++++++++" + string.Join(", ", roles));

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
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
        //otp session
        [HttpGet]
        public IActionResult VerifyOTP()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyOTP(OTPViewModel model)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");
            var userId = HttpContext.Session.GetString("RegisterUserId");

            if (model.OTP == storedOtp && !string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("UserLogin", "FinanceUser");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid OTP.");
            return View(model);
        }
        //forgototp
        [HttpGet]
        public async Task<IActionResult> VerifyForgotOTP()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyForgotOTP(OTPViewModel model)
        {
            var storedOtp = HttpContext.Session.GetString("ForgotOTP");
            var userId = HttpContext.Session.GetString("ForgotUserId");

            if (model.OTP == storedOtp && !string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    return RedirectToAction("UserResetPassword","FinanceUser");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid OTP.");
            return View(model);
        }
        //Reset Password
        [HttpGet]
        public async Task<IActionResult> UserResetPassword(String UserId)
        {
            return View(UserId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserResetPassword(UserResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) 
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && user.ForgotPassword == false) {

                var result = await _userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                result = await _userManager.AddPasswordAsync(user, model.Password); // Set new password
                if (result.Succeeded)
                {
                    user.ForgotPassword = false;
                    _context.financeusers.Update(user);
                    _context.SaveChanges();
                    return RedirectToAction("UserLogin", "FinanceUser");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        //resendOtp function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendOTP(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var otp = GenerateOTP();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("RegisterUserId", user.Id);

            await _emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your new OTP code is {otp}");

            return RedirectToAction("VerifyOTP");
        }
    }
}
