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
        private static EventLogger _eventLogger;

        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        /// <summary>
        /// Initialization Telegram Bot
        /// </summary>
        /// <param name="logger">EventLogger</param>
        /// <returns>None</returns>
        public static async Task Initialization(EventLogger logger)
        {
            if(_botClient != null)
                return;
            
            _eventLogger = logger;
            _eventLogger.AddClass<Bot>();
            
            InitCommandsList();

            try
            {
                if (string.IsNullOrWhiteSpace(AppSettings.ProxyHost) && AppSettings.ProxyPort == 0)
                {
                    _botClient = new TelegramBotClient(AppSettings.TelegramToken);
                    
                    _eventLogger.LogInfo("Bot using only WebHook");
                }
                else
                {
                    // Using Proxy
                    var proxy = new HttpToSocks5Proxy(AppSettings.ProxyHost, AppSettings.ProxyPort);
                    _botClient = new TelegramBotClient(AppSettings.TelegramToken, proxy);

                    _eventLogger.LogInfo("Bot using WebHook and proxy");
                }

                // Using WebHook
                var url = Environment.GetEnvironmentVariable("URL_ENVIRONMENT");
                var hook = $"{url}/api/message/update";
                await _botClient.SetWebhookAsync(hook);

                // Logging
                var me = _botClient.GetMeAsync().Result;
                _eventLogger.LogDebug($"Bot id {me.Id}. Bot name {me.FirstName}");

            }
            catch (Exception e)
            {
                _eventLogger.LogError(e);
            }
        }
        
        /// <summary>
        /// Getting telegram bot client
        /// </summary>
        /// <returns>Telegram Bot</returns>
        public static TelegramBotClient GetBotClient()
        {
            return _botClient;
        }

        // Initialization commands
        private static void InitCommandsList()
        {
            _commandsList = new List<Command>();
            _commandsList.Add(new StartCommand(_eventLogger));
            //TODO: Add more commands
        }
    }
}