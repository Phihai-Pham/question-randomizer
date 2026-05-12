using Microsoft.AspNetCore.Mvc;

namespace QuestionRandomizerApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}