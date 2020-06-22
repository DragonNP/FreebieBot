using System.Collections.Generic;
using System.Linq;
using FreebieBot.Models.Users;
using FreebieBot.Models.Logger;
using FreebieBot.Models.Translates;
using Telegram.Bot;

namespace FreebieBot.Models.TelegramBot
{
    public class SendTelegramMessage
    {
        private static readonly TelegramBotClient Bot = TelegramBot.Bot.GetBotClient();
        private static EventLogger _eventLogger;

        public SendTelegramMessage(EventLogger eventLogger)
        {
            _eventLogger = eventLogger;
            _eventLogger.AddClass<SendTelegramMessage>();
        }
        
        /// <summary>
        /// Sending more message
        /// </summary>
        /// <param name="users">Users to send a message to</param>
        /// <param name="lines">List of message</param>
        public void SendMore(IQueryable<User> users, List<Line> lines)
        {
            foreach (var user in users)
            {
                foreach (var line in lines)
                {
                    _eventLogger.LogDebug($"Send text message to {user.TelegramId}");
                    Bot.SendTextMessageAsync(user.TelegramId, user.Lang == UserLang.ru ? line.LineRus : line.Default);
                }
            }
        }
    }
}