using System;
using System.Collections.Generic;
using FreebieBot.Models;
using FreebieBot.Models.TelegramBot.Commands;
using FreebieBot.Models.TelegramBot.TextCommands;
using MihaZupan;
using Telegram.Bot;

namespace FreebieBot.Services
{
    public class TelegramBotService
    {
        private static TelegramBotClient _botClient;
        private static List<Command> _commandsList;
        private static List<TextCommand> _textCommandsList;
        private static EventLoggerService _eventLogger;

        public IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();
        public IReadOnlyList<TextCommand> TextCommands => _textCommandsList.AsReadOnly();

        /// <summary>
        /// Initialization Telegram Bot
        /// </summary>
        /// <param name="logger">EventLogger</param>
        /// <returns>None</returns>
        public TelegramBotService(EventLoggerService logger)
        {
            if(_botClient != null)
                return;
            
            _eventLogger = logger;
            _eventLogger.AddClass<TelegramBotService>();
            
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
                _botClient.SetWebhookAsync(hook);

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
        public TelegramBotClient GetBotClient()
        {
            if (_botClient == null)
                throw new Exception("Telegram bot service not initialized");
            return _botClient;
        }

        // Initialization commands
        private static void InitCommandsList()
        {
            _commandsList = new List<Command>();
            _commandsList.Add(new StartCommand());
            
            _textCommandsList = new List<TextCommand>();
            _textCommandsList.Add(new BackTextCommand());
            _textCommandsList.Add(new SettingsCommand());
            _textCommandsList.Add(new SubscribePikabuCommand());
            _textCommandsList.Add(new UnsubscribePikabuCommand());
            _textCommandsList.Add(new DelAccountTextCommands());
            _textCommandsList.Add(new LanguagesTextCommand());
        }
    }
}