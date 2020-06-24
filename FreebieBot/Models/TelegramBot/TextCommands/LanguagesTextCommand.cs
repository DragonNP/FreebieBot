using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Translates;
using FreebieBot.Models.Users;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public class LanguagesTextCommand : TextCommand
    {
        protected override Line Name { get; set; }
        private List<string> Langs { get; set; } = new List<string>();
        private Line ChooseLang { get; set; }
        private Line LangChanged { get; set; }
        
        public override async Task<bool> Contains(Message message, ApplicationContext context)
        {
            if (message.Type != MessageType.Text)
                return false;

            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            // If user not registered
            if (user == null) return false;

            if (Name == null)
                Name = await context.Lines.FindAsync("changeLanguage");
            
            if (Langs.Count == 0)
            {
                foreach (UserLang lang in Enum.GetValues(typeof(UserLang)))
                    Langs.Add(lang.ToString());
            }
            return Name.Contains(message.Text);
        }

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups)
        {
            if (Langs.Contains(message.Text))
            {
                await SetLanguage(message, client, context, markups);
                return;
            }

            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return;
            
            if (ChooseLang == null)
                ChooseLang = await context.Lines.FindAsync("chooseLang");
            var chooseLang = ChooseLang.GetTranslate(user.Lang);

            await client.SendTextMessageAsync(user.TelegramId, chooseLang, replyMarkup: markups.GetLanguageMarkup());
        }

        private async Task SetLanguage(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return;

            Enum.TryParse(message.Text, out UserLang lang);
            user.Lang = lang;
            context.Users.Update(user);

            if (LangChanged == null)
                LangChanged = await context.Lines.FindAsync("langChanged");
            var langChanged = LangChanged.GetTranslate(user.Lang);
            
            await client.SendTextMessageAsync(user.TelegramId, langChanged, replyMarkup: markups.GetSettingsMarkup(user));
            await context.SaveChangesAsync();
        }
    }
}