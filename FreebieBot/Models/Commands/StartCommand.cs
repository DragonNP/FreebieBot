using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreebieBot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient, DatabaseContext db)
        {
            var chat = message.Chat;
            var lang = message.From.LanguageCode;
            
            // Add new user
            var newUser = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = lang};
            var line = db.Lines.Find("hello");
            var helloText = lang == "ru" ? line.LineRus : line.Default;
            var isNew = !db.Users.Any(p => p.TelegramId == chat.Id);
            
            if (isNew)
            {
                await db.Users.AddAsync(newUser);
                await db.SaveChangesAsync();
            }
            
            await botClient.SendTextMessageAsync(chat.Id, string.Format(helloText, newUser.Name));
        }
    }
}