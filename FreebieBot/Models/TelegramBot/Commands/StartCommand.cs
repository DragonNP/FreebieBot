using System;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Users;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = FreebieBot.Models.Users.User;

namespace FreebieBot.Models.TelegramBot.Commands
{
    public class StartCommand : Command
    {
        private readonly EventLoggerService _eventLogger;
        public override string Name => @"/start";

        public StartCommand(EventLoggerService eventLogger)
        {
            _eventLogger = eventLogger;
        }

        /// <summary>
        /// If text from message == name command
        /// </summary>
        /// <param name="message">Message from user</param>
        /// <returns></returns>
        public override bool Contains(Message message)
        {
            if (message.Type != MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }
        
        /// <summary>
        /// Executing command 
        /// </summary>
        /// <param name="message">Message from user</param>
        /// <param name="botClient">Bot Client</param>
        /// <param name="context">DB Context</param>
        /// <returns></returns>
        public override async Task Execute(Message message, TelegramBotClient botClient, ApplicationContext context)
        {
            try
            {
                var chat = message.Chat;
                var lang = message.From.LanguageCode;

                Enum.TryParse(lang, out UserLang userLang);

                // Add new user
                var newUser = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = userLang};
                var line = context.Lines.Find("hello");
                var helloText = lang == "ru" ? line.LineRus : line.Default;
                var isNew = !context.Users.Any(p => p.TelegramId == chat.Id);
                
                if (isNew)
                {
                    await context.Users.AddAsync(newUser);
                    await context.SaveChangesAsync();
                }
                
                await botClient.SendTextMessageAsync(chat.Id, string.Format(helloText, newUser.Name));
            }
            catch (Exception e)
            {
                _eventLogger.LogError(e, Convert.ToString(message.Chat.Id));
            }
        }
    }
}