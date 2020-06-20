using System;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;
using FreebieBot.Models.TelegramBot;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace FreebieBot.Controllers
{
    [Route("/api/message/update")]
    public class MessageController : Controller
    {
        private static readonly EventLogger EventLogger = Log.Logger;
        private readonly DatabaseContext _db;

        public MessageController(DatabaseContext db)
        {
            EventLogger.AddClass<MessageController>();
            EventLogger.LogDebug("Initialization MessageController");
            
            _db = db;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null) return Ok();
            var message = update.Message;
            
            try
            {
                var commands = Bot.Commands;
                var botClient = Bot.GetBotClientAsync();

                EventLogger.LogDebug($"Received {message.Type}", message.From.Id.ToString());

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
                EventLogger.LogError(e, message.From.Id.ToString());
            }
            
            return Ok();
        }
    }
}