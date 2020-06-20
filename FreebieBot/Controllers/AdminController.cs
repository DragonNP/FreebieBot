using System;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreebieBot.Controllers
{
    public class AdminController : Controller
    {
        private readonly EventLogger _eventLogger;
        private readonly DatabaseContext _db;

        public AdminController(DatabaseContext db, EventLogger eventLogger)
        {
            _eventLogger = eventLogger;
            _eventLogger.AddClass<AdminController>();
            _eventLogger.LogDebug("Initialization AdminController");
            
            _db = db;
        }
        
        [HttpGet]
        public async Task<IActionResult> Translate()
        {
            _eventLogger.LogDebug("/Home/TranslateView page visited");
            return View(await _db.Lines.ToListAsync());
        }
        
        [HttpGet]
        public IActionResult TranslateAdd()
        {
            _eventLogger.LogDebug("/Home/TranslateAdd page visited");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> TranslateAdd(Line line)
        {
            _eventLogger.LogInfo("Adding new translate");
            
            _db.Lines.Add(line);
            await _db.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpPost]
        public async Task<IActionResult> TranslateDelete(string name)
        {
            _eventLogger.LogInfo("Deleting translate");
            
            var line = await _db.Lines.FindAsync(name);

            if (line == null)
            {
                return RedirectToAction("Translate");
            }

            _db.Lines.Remove(line);
            await _db.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpGet]
        public async Task<IActionResult> TranslateEdit(string name)
        {
            _eventLogger.LogDebug("/Home/TranslateEdit page visited");
            
            var line = await _db.Lines.FindAsync(name);
            
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
            
            _db.Lines.Update(line);
            await _db.SaveChangesAsync();
            return RedirectToAction("Translate");
        }
        
        [HttpGet]
        public async Task<IActionResult> EventLog()
        {
            _eventLogger.LogDebug("/Home/EventLog page visited");
            return View(await _db.EventLogs.ToListAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> EventLogClear()
        {
            _eventLogger.LogInfo("Clearing Event Logs");

            await _db.Database.ExecuteSqlRawAsync("Truncate table EventLogs");
            return RedirectToAction("EventLog");
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            _eventLogger.LogDebug("/Home/Users page visited");
            return View(await _db.Users.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit(string id)
        {
            _eventLogger.LogDebug("/Home/UserEdit page visited", id);
            
            var user = await _db.Users.FindAsync(Convert.ToInt64(id));

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
            
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Users");
        }
        
        [HttpPost]
        public async Task<IActionResult> UserDelete(string id)
        {
            _eventLogger.LogInfo("Deleting User", id);
            
            var user = await _db.Users.FindAsync(Convert.ToInt64(id));

            if (user == null)
            {
                return RedirectToAction("Users");
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Users");
        }
    }
}