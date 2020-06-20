using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreebieBot.Models.Logger;
using FreebieBot.Models.TelegramBot.Commands;
using MihaZupan;
using Telegram.Bot;

namespace FreebieBot.Models.TelegramBot
{
    public class Bot
    {
        private static TelegramBotClient _botClient;
        private static List<Command> _commandsList;
        private static readonly EventLogger EventLogger = Log.Logger;

        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if(_botClient != null)
                return _botClient;
            
            EventLogger.AddClass<Bot>();
            InitCommandsList();

            try
            {
                if (string.IsNullOrWhiteSpace(AppSettings.ProxyHost) && AppSettings.ProxyPort == 0)
                {
                    _botClient = new TelegramBotClient(AppSettings.TelegramToken);
                    
                    EventLogger.LogInfo("Bot using only WebHook");
                }
                else
                {
                    var proxy = new HttpToSocks5Proxy(AppSettings.ProxyHost, AppSettings.ProxyPort);
                    _botClient = new TelegramBotClient(AppSettings.TelegramToken, proxy);

                    EventLogger.LogInfo("Bot using WebHook and proxy");
                }

                // WebHook
                var url = Environment.GetEnvironmentVariable("URL_ENVIRONMENT");
                var hook = $"{url}/api/message/update";
                await _botClient.SetWebhookAsync(hook);

                // Logging
                var me = _botClient.GetMeAsync().Result;
                EventLogger.LogDebug($"Bot id {me.Id}. Bot name {me.FirstName}");

                return _botClient;
            }
            catch (Exception e)
            {
                EventLogger.LogError(e);
            }

            return null;
        }

        private static void InitCommandsList()
        {
            _commandsList = new List<Command>();
            _commandsList.Add(new StartCommand(EventLogger));
            //TODO: Add more commands
        }
    }
}