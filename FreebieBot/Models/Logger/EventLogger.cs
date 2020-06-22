using System;
using System.Runtime.CompilerServices;
using FreebieBot.Models.Database;

namespace FreebieBot.Models.Logger
{
    public class EventLogger
    {
        private ApplicationContext _db;
        private LoggerLevel _level;
        private string _className;

        public EventLogger(ApplicationContext db)
        {
            _db = db;
            Enum.TryParse(AppSettings.LoggerLevel, out _level);
        }

        public void AddClass<T>()
            where T : class
        {
            _className = typeof(T).Name;
        }

        public void LogError(Exception e, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.fail < _level)
                return;
            
            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.fail, e.Message, location, userId);
            SaveToDatabase("Error", e.Message, location, e.StackTrace, userId);
        }

        public void LogInfo(string message, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.info < _level)
                return;
            
            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.info, message, location, userId);
            SaveToDatabase("Information", message, location, userId: userId);
        }

        public void LogDebug(string message, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.dbug < _level)
                return;

            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.dbug, message, location, userId);
            SaveToDatabase("Debug", message, location, userId: userId);
        }

        private void PrintLog(LoggerLevel level, string message, string location, string userId = null)
        {
            ConsoleColor foregroundColor, backgroundColor;
            switch (level)
            {
                case LoggerLevel.dbug:
                    foregroundColor = ConsoleColor.Gray;
                    backgroundColor = default;
                    break;
                case LoggerLevel.info:
                    foregroundColor = ConsoleColor.Green;
                    backgroundColor = default;
                    break;
                case LoggerLevel.fail:
                    foregroundColor = ConsoleColor.White;
                    backgroundColor = ConsoleColor.Red;
                    break;
                case LoggerLevel.crit:
                    foregroundColor = ConsoleColor.Black;
                    backgroundColor = ConsoleColor.Red;
                    break;
                default:
                    foregroundColor = default;
                    backgroundColor = default;
                    break;
            }
            
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(level.ToString());
            Console.ResetColor();
            Console.Write(": ");

            Console.WriteLine(location);
            Console.Write($"\t{message}");

            if (!string.IsNullOrEmpty(userId))
                Console.WriteLine($"\tuserId: {userId}");
            else
                Console.WriteLine();
        }

        private void SaveToDatabase(string level, string message, string location,
            string stackTrace = null, string userId = null)
        {
            _db.EventLogs.Add(new EventLog()
            {
                Type = level,
                Message = message,
                Location = location,
                StackTrace = stackTrace,
                UserID = userId,
                DateTime = DateTime.Now
            });
            _db.SaveChanges();
        }
    }
}