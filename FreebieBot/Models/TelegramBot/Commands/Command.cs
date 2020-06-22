using System.Threading.Tasks;
using FreebieBot.Models.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreebieBot.Models.TelegramBot.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract bool Contains(Message message);
        public abstract Task Execute(Message message, TelegramBotClient client, ApplicationContext context);

    }
}