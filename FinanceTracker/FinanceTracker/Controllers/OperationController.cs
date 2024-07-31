using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    public class OperationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
