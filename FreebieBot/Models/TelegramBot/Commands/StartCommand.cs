using System;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = FreebieBot.Models.Database.User;

namespace FreebieBot.Models.TelegramBot.Commands
{
    public class StartCommand : Command
    {
        private readonly EventLogService _eventLogger;
        public override string Name => @"/start";

        public StartCommand(EventLogService eventLogger)
        {
            _eventLogger = eventLogger;
        }

        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient, DatabaseContext db)
        {
            try
            {
                var chat = message.Chat;
                var lang = message.From.LanguageCode;

                Enum.TryParse(lang, out UserLang userLang);

                // Add new user
                var newUser = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = userLang};
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
            catch (Exception e)
            {
                _eventLogger.LogError(e, "StartCommand -> Execute", Convert.ToString(message.Chat.Id));
            }
        }
    }
}