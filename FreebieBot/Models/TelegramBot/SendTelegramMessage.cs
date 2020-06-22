using System.Linq;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;
using Telegram.Bot;

namespace FreebieBot.Models.TelegramBot
{
    public static class SendTelegramMessage
    {
        private static readonly TelegramBotClient Bot = TelegramBot.Bot.GetBotClientAsync().Result;
        private static readonly EventLogger EventLogger = Log.Logger;
        
        public static void Sends(IQueryable<User> users, Line line)
        {
            foreach (var user in users)
            {
                EventLogger.LogDebug($"Send text message to {user.TelegramId}");
                Bot.SendTextMessageAsync(user.TelegramId, user.Lang == UserLang.ru ? line.LineRus : line.Default);
            }
        }
    }
}