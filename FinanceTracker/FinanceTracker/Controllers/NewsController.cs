using FinanceTracker.DbClass;
using FinanceTracker.Models.News;
using FinanceTracker.Models.User;
using FinanceTracker.ViewModels.NewsView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
        [HttpGet]
        public async Task<IActionResult> OneNews(int newsid)
        {
            var onenew = await _context.news.FirstOrDefaultAsync(n => n.NewsId == newsid);
            var comment = await _context.commentss.Where(com => com.newsid == onenew.NewsId).ToListAsync();
            var newmodel = new NewsViewModel
            {
                news = onenew,
                Comments = comment
            };
            return View(newmodel);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(NewsViewModel model)
        {
            var onenew = await _context.news.FirstOrDefaultAsync(n => n.NewsId == model.news.NewsId);
            var comment = new CommentModel
            {
                CommentBody = model.cbody,
                newsid = onenew.NewsId,
                userid = (await _userManager.GetUserAsync(User))?.Id,
                username = (await _userManager.GetUserAsync(User))?.UserName,
            };
            _context.commentss.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("OneNews", "News", new {newsid = onenew.NewsId});
        }
    }
}
