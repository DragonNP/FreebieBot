using System;

namespace FreebieBot.Models
{
    public static class AppSettings
    {
        public static string UrlDatabase { get; } = Environment.GetEnvironmentVariable("URL_DATABASE");
        
        public static string LoggerLevel { get; } = Environment.GetEnvironmentVariable("LOGGER_LEVEL");
        public static string TelegramToken { get; } = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
        public static string ProxyHost { get; } = Environment.GetEnvironmentVariable("PROXY_HOST");
        public static int ProxyPort { get; } = Convert.ToInt32(Environment.GetEnvironmentVariable("PROXY_PORT"));
    }
}