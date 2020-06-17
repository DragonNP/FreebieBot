using System;
using System.Collections.Generic;
using FreebieBot.Models.Commands;
using Microsoft.Extensions.Logging;
using MihaZupan;
using Telegram.Bot;

namespace FreebieBot.Models
{
    public class Bot
    {
        
        private static TelegramBotClient _botClient;
        private static List<Command> _commandsList;
        private static readonly ILogger Logger = Log.CreateLogger<Bot>();

        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        private static void InitCommandsList()
        {
            _commandsList = new List<Command>();
            _commandsList.Add(new StartCommand());
            //TODO: Add more commands
        }

        public static TelegramBotClient GetBotClient()
        {
            return _botClient;
        }

        public static async void InitBotClient(bool isDevelopment)
        {
            InitCommandsList();
            
            string telegramToken  = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
            
            if (isDevelopment)
            {
                // Proxy
                string proxyHost = Environment.GetEnvironmentVariable("PROXY_HOST");
                int proxyPort = Convert.ToInt32(Environment.GetEnvironmentVariable("PROXY_PORT"));
                HttpToSocks5Proxy proxy = new HttpToSocks5Proxy(proxyHost, proxyPort);
                
                _botClient = new TelegramBotClient(telegramToken, proxy);
                
                Logger.LogInformation($"Bot using WebHook and proxy");
            }
            else
            {
                _botClient = new TelegramBotClient(telegramToken);
                
                Logger.LogInformation($"Bot using only WebHook");
            }
            
            // WebHook
            string url  = Environment.GetEnvironmentVariable("URL_ENVIRONMENT");
            string hook = $"{url}/api/message/update";
            await _botClient.SetWebhookAsync(hook);

            // Logging
            var me = _botClient.GetMeAsync().Result;
            Logger.LogDebug($"Bot id {me.Id}. Bot name {me.FirstName}");
        }
    }
}