using System;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace FreebieBot.Controllers
{
    [Route("/api/message/update")]
    public class MessageController : Controller
    {
        private readonly EventLoggerService _eventLogger;
        private readonly ApplicationContext _context;
        private readonly TelegramBotService _telegramBot;
        private readonly TelegramMarkupsService _markups;

        public MessageController(ApplicationContext context, EventLoggerService eventLogger, TelegramBotService telegramBot,
            TelegramMarkupsService markups)
        {
            _eventLogger = eventLogger;
            _context = context;
            _telegramBot = telegramBot;
            _markups = markups;

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
                var commands = _telegramBot.Commands;
                var textCommands = _telegramBot.TextCommands;
                var telegramBot = _telegramBot.GetBotClient();

                _eventLogger.LogDebug($"Received {message.Type}", message.From.Id.ToString());

                foreach (var command in commands)
                {
                    if (!command.Contains(message)) continue;

                    
                    await command.Execute(message, telegramBot, _context, _markups);
                    return Ok();
                }

                foreach (var textCommand in textCommands)
                {
                    var contains = !(await textCommand.Contains(message, _context));
                    if (contains) continue;
                    
                    await textCommand.Execute(message, telegramBot, _context, _markups);
                    return Ok();
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