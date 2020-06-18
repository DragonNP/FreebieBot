using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FreebieBot.Models;
using Microsoft.EntityFrameworkCore;

namespace FreebieBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _db;

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public async Task<IActionResult> Users()
        {
            return View(await _db.Users.ToListAsync());
        }
        
        public async Task<IActionResult> TranslateView()
        {
            return View(await _db.Lines.ToListAsync());
        }
        
        public IActionResult TranslateAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TranslateSave(Line line)
        {
            _db.Lines.Add(line);
            await _db.SaveChangesAsync();
            return RedirectToAction("TranslateView");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}