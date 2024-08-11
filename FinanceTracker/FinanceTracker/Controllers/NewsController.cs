using FinanceTracker.DbClass;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Controllers
{
    public class NewsController : Controller
    {
        private readonly FinanceDbContext _context;

        public NewsController(FinanceDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> ShowNews()
        {
            var news = await _context.news.ToListAsync();
            return View(news);
        }

    }
}
