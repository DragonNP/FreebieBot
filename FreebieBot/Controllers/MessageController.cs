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
        private readonly EventLogger _eventLogger;
        private readonly ApplicationContext _context;

        public MessageController(ApplicationContext context, EventLogger eventLogger)
        {
            _eventLogger = eventLogger;
            _context = context;

            _eventLogger.AddClass<MessageController>();
            _eventLogger.LogDebug("Initialization MessageController");
            
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null) return Ok();
            var message = update.Message;
            
            try
            {
                var commands = Bot.Commands;
                var botClient = Bot.GetBotClient();

                _eventLogger.LogDebug($"Received {message.Type}", message.From.Id.ToString());

                foreach (var command in commands)
                {
                    if (command.Contains(message))
                    {
                        await command.Execute(message, botClient, _context);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _eventLogger.LogError(e, message.From.Id.ToString());
            }
            
            return Ok();
        }
    }
}