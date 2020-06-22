using System;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Users;
using FreebieBot.Models.Translates;
using FreebieBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreebieBot.Controllers
{
    public class AdminController : Controller
    {
        private readonly EventLoggerService _eventLogger;
        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context, EventLoggerService eventLogger)
        {
            _eventLogger = eventLogger;
            _eventLogger.AddClass<AdminController>();
            _eventLogger.LogDebug("Initialization AdminController");
            
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> Translate()
        {
            _eventLogger.LogDebug("/Admin/TranslateView page visited");
            return View(await _context.Lines.ToListAsync());
        }
        
        [HttpGet]
        public IActionResult TranslateAdd()
        {
            _eventLogger.LogDebug("/Admin/TranslateAdd page visited");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> TranslateAdd(Line line)
        {
            _eventLogger.LogInfo("Adding new translate");
            
            _context.Lines.Add(line);
            await _context.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpPost]
        public async Task<IActionResult> TranslateDelete(string name)
        {
            _eventLogger.LogInfo("Deleting translate");
            
            var line = await _context.Lines.FindAsync(name);

            if (line == null)
            {
                return RedirectToAction("Translate");
            }

            _context.Lines.Remove(line);
            await _context.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpGet]
        public async Task<IActionResult> TranslateEdit(string name)
        {
            _eventLogger.LogDebug("/Admin/TranslateEdit page visited");
            
            var line = await _context.Lines.FindAsync(name);
            
            if (line == null)
            {
                return RedirectToAction("Translate");
            }
            
            return View(line);
        }
        
        [HttpPost]
        public async Task<IActionResult> TranslateEdit(Line line)
        {
            _eventLogger.LogInfo("Editing translate");
            
            _context.Lines.Update(line);
            await _context.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpGet]
        public async Task<IActionResult> EventLog()
        {
            _eventLogger.LogDebug("/Admin/EventLog page visited");
            return View(await _context.EventLogs.ToListAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> EventLogClear()
        {
            _eventLogger.LogInfo("Clearing Event Logs");

            await _context.Database.ExecuteSqlRawAsync("Truncate table EventLogs");
            return RedirectToAction("EventLog");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            _eventLogger.LogDebug("/Admin/Users page visited");

            var users = await _context.Users.ToListAsync();
            
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            _eventLogger.LogDebug("/Admin/UserEdit page visited", id);
            
            var user = await _context.Users.FindAsync(Convert.ToInt64(id));

            if (user == null)
            {
                return RedirectToAction("Users");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(User user)
        {
            _eventLogger.LogInfo("Editing User", user.Id.ToString());
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }
        
        [HttpPost]
        public async Task<IActionResult> UserDelete(string id)
        {
            _eventLogger.LogInfo("Deleting User", id);
            
            var user = await _context.Users.FindAsync(Convert.ToInt64(id));

            if (user == null)
            {
                return RedirectToAction("Users");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            _eventLogger.LogDebug("/Admin/Posts page visited");
            
            var posts = await _context.Posts.ToListAsync();
            
            return View(posts);
        }
    }
}