using System.Threading.Tasks;
using FreebieBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace FreebieBot.Controllers
{
    [Route("/api/message/update")]
    public class MessageController : Controller
    {
        private static readonly ILogger Logger = Log.CreateLogger<Bot>();
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            if (update == null) return Ok();
            
            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = Bot.GetBotClient();
            
            Logger.LogDebug($"Received {message.Type} from user id: {message.From.Id}");

            foreach (var command in commands)
            {
                if (command.Contains(message))
                {
                    await command.Execute(message, botClient);
                    break;
                }
            }
            return Ok();
        }
    }
}