using Microsoft.AspNetCore.Mvc;
using FreebieBot.Services;

namespace FreebieBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventLogService _evenLogger;

        public HomeController(EventLogService evenLogger)
        {
            _evenLogger = evenLogger;
        }

        public IActionResult Index()
        {
            _evenLogger.LogDebug("/Home/Index page visited", "HomeController -> Index");
            
            return View();
        }

        public IActionResult Privacy()
        {
            _evenLogger.LogDebug("/Home/Privacy page visited", "HomeController -> Privacy");
            
            return View();
        }
    }
}