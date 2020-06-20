using System;
using System.Threading.Tasks;
using FreebieBot.Models;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;
using FreebieBot.Models.TelegramBot;
using FreebieBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace FreebieBot.Controllers
{
    [Route("/api/message/update")]
    public class MessageController : Controller
    {
        private static readonly EventLogService EventLogger = Log.EventLogger;
        private readonly DatabaseContext _db;

        public MessageController(DatabaseContext db)
        {
            EventLogger.LogDebug("Initialization MessageController", "MessageController -> Init");
            
            _db = db;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            try
            {
                if (update == null) return Ok();

                var commands = Bot.Commands;
                var message = update.Message;
                var botClient = Bot.GetBotClientAsync();

                EventLogger.LogDebug($"Received {message.Type} from user id: {message.From.Id}",
                    "MessageController -> Post");

                foreach (var command in commands)
                {
                    if (command.Contains(message))
                    {
                        await command.Execute(message, await botClient, _db);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.LogError(e, "MessageController -> Post");
            }
            return Ok();
        }
    }
}