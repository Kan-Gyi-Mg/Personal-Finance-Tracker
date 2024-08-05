﻿using FinanceTracker.DbClass;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.FinanceUserView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanceTracker.RoleInitiator;

namespace FinanceTracker.Controllers
{
    public class FinanceUserController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        public FinanceUserController(EmailService emailService,SignInManager<FinanceUser> signInManager,UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        //show all user
        [HttpGet]
        public async Task<IActionResult> ShowUserList()
        {
            var User = _userManager.Users;
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
