using FinanceTracker.DbClass;
using FinanceTracker.Models.News;
using FinanceTracker.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Controllers
{
    public class NewsController : Controller
    {
        private readonly EmailService _emailService;
        private readonly SignInManager<FinanceUser> _signInManager;
        private readonly UserManager<FinanceUser> _userManager;
        private readonly FinanceDbContext _context;
        public NewsController(EmailService emailService, SignInManager<FinanceUser> signInManager, UserManager<FinanceUser> userManager, FinanceDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        [HttpGet]
        public async Task<IActionResult> ShowNews()
        {
            var news = await _context.news.ToListAsync();
            return View(news);
        }
        [HttpGet]
        public async Task<IActionResult> AddNews()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNews(NewsModel model)
        {
            FinanceUser user = await _userManager.GetUserAsync(User);
            var addnews = new NewsModel
            {
                NewsTitle = model.NewsTitle,
                NewsBody = model.NewsBody,
                Description = model.Description,
                UserId = user.Id
            };
            await _context.news.AddAsync(addnews);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowNews","News");
        }
        [HttpGet]
        public async Task<IActionResult> EditNews(int newsid)
        {
            var newlist = await _context.news.FirstOrDefaultAsync(n => n.NewsId == newsid);
            return View(newlist);
        }
        [HttpPost]
        public async Task<IActionResult> EditNews(NewsModel model)
        {
            Console.WriteLine("+++++++++++++++" + model.NewsId);
            var editnew = await _context.news.FirstOrDefaultAsync(n => n.NewsId == model.NewsId);
            editnew.NewsTitle = model.NewsTitle;
            editnew.Description = model.Description;
            editnew.NewsBody = model.NewsBody;
            _context.news.Update(editnew);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowNews", "News");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNews(int newsid)
        {
            var newlist = await _context.news.FirstOrDefaultAsync(n => n.NewsId == newsid);
            _context.news.Remove(newlist);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowNews", "News");
        }

    }
}
