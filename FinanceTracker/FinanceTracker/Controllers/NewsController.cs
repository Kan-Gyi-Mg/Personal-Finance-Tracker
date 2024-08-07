using FinanceTracker.DbClass;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }

    }
}
