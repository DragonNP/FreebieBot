using System.Collections.Generic;
using FreebieBot.Models.Database;
using FreebieBot.Models.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FreebieBot.Services
{
    public class TelegramMarkupsService
    {
        private readonly ApplicationContext _context;

        public TelegramMarkupsService(ApplicationContext context)
        {
            _context = context;
        }

        public IReplyMarkup GetMainMarkup(UserLang lang)
        {
            var keyboard = new List<List<KeyboardButton>>();
            var line1 = new List<KeyboardButton>();
            var settings = _context.Lines.Find("settings").GetTranslate(lang);
            line1.Add(settings);
            keyboard.Add(line1);

            IReplyMarkup mainMarkup = new ReplyKeyboardMarkup(keyboard, true);
            return mainMarkup;
        }

        public IReplyMarkup GetSettingsMarkup(User user)
        {
            var keyboard = new List<List<KeyboardButton>>();
            
            var line = new List<KeyboardButton>();
            var paramLine = user.SubPikabu == UserSub.Yes ? "unsubscribeFrom" : "subscribeTo";
            var subUnSubPikabu = string.Format(_context.Lines.Find(paramLine).GetTranslate(user.Lang), "Pikabu");
            line.Add(subUnSubPikabu);
            keyboard.Add(line);

            var line2 = new List<KeyboardButton>();
            paramLine = "back";
            var back = _context.Lines.Find(paramLine).GetTranslate(user.Lang);
            line2.Add(back);
            paramLine = "deleteAccount";
            var deleteAccount = _context.Lines.Find(paramLine).GetTranslate(user.Lang);
            line2.Add(deleteAccount);
            
            keyboard.Add(line2);

            IReplyMarkup settingsMarkup = new ReplyKeyboardMarkup(keyboard, true);
            return settingsMarkup;
        }

        public IReplyMarkup GetStartMarkup()
        {
            var keyboard = new List<List<KeyboardButton>>();
            var line1 = new List<KeyboardButton>();
            line1.Add("/start");
            keyboard.Add(line1);
            
            return new ReplyKeyboardMarkup(keyboard, true);
        }
    }
}