using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers
{
    public class OperationController : Controller
    {
        //landing page of the site
        public IActionResult StartingPage()
        {
            return View();
        }
    }
}
