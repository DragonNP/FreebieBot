using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Translates;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public abstract class TextCommand
    {
        protected abstract Line Name { get; set; }
        public abstract Task<bool> Contains(Message message, ApplicationContext context);
        public abstract Task Execute(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups);

    }
}