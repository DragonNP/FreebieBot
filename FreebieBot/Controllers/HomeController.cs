using FreebieBot.Models.Logger;
using Microsoft.AspNetCore.Mvc;

namespace FreebieBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventLogger _eventLogger;

        public HomeController(EventLogger eventLogger)
        {
            _eventLogger = eventLogger;
            _eventLogger.AddClass<HomeController>();
        }

        public IActionResult Index()
        {
            _eventLogger.LogDebug("/Home/Index page visited");
            
            return View();
        }

        public IActionResult Privacy()
        {
            _eventLogger.LogDebug("/Home/Privacy page visited");
            
            return View();
        }
    }
}