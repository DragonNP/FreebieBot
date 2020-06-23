using System;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Translates;
using FreebieBot.Models.Users;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = FreebieBot.Models.Users.User;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public class SettingsCommand : TextCommand
    {
        protected override Line Name { get; set; }
        
        public override async Task<bool> Contains(Message message, ApplicationContext context)
        {
            if (message.Type != MessageType.Text)
                return false;

            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            // If user not registered
            if (user == null)
            {
                var lang = message.From.LanguageCode;

                Enum.TryParse(lang, out UserLang userLang);
                
                user = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = userLang};
                await context.Users.AddAsync(user);
            }

            if (Name == null)
                Name = await context.Lines.FindAsync("settings");
            var settings = Name.GetTranslate(user.Lang);
            return message.Text.Contains(settings);
        }

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null)
            {
                var lang = message.From.LanguageCode;

                Enum.TryParse(lang, out UserLang userLang);
                
                user = new User() {TelegramId = chat.Id, Name = chat.FirstName, Lang = userLang};
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            
            if (Name == null)
                Name = await context.Lines.FindAsync("settings");
            var settings = Name.GetTranslate(user.Lang);

            await client.SendTextMessageAsync(user.TelegramId, settings, replyMarkup: markups.GetSettingsMarkup(user));
        }
    }
}