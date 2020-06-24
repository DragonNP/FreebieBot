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
        public override string Name { get; } = @"/start";

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
        /// <param name="markups">Keyboards</param>
        /// <returns></returns>
        public override async Task Execute(Message message, TelegramBotClient botClient, ApplicationContext context,
            TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var lang = message.From.LanguageCode;

            Enum.TryParse(lang, out UserLang userLang);


            var line = await context.Lines.FindAsync("hello");
            var helloText = line.GetTranslate(userLang);

            // Add new user
            var newUser = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = userLang};
            var isNew = !context.Users.Any(p => p.TelegramId == chat.Id);

            if (isNew)
            {
                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();
            }

            await botClient.SendTextMessageAsync(chat.Id, string.Format(helloText, newUser.Name),
                replyMarkup: markups.GetMainMarkup(userLang));
        }
    }
}