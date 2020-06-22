using System;
using System.Runtime.CompilerServices;
using FreebieBot.Models;
using FreebieBot.Models.Database;
using FreebieBot.Models.EventLogs;

namespace FreebieBot.Services
{
    public class EventLoggerService
    {
        private readonly ApplicationContext _db;
        private readonly LoggerLevel _level;
        private string _className;

        public EventLoggerService(ApplicationContext db)
        {
            _db = db;
            Enum.TryParse(AppSettings.LoggerLevel, out _level);
        }

        /// <summary>
        /// Adding Class for logging
        /// </summary>
        /// <typeparam name="T">Class</typeparam>
        public void AddClass<T>()
            where T : class
        {
            _className = typeof(T).Name;
        }

        /// <summary>
        /// Logging error
        /// </summary>
        /// <param name="e">Error</param>
        /// <param name="userId">User Id (optional)</param>
        /// <param name="methodName">Name caller method (optional)</param>
        public void LogError(Exception e, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.fail < _level)
                return;
            
            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.fail, e.Message, location, userId);
            SaveToDatabase("Error", e.Message, location, e.StackTrace, userId);
        }

        /// <summary>
        /// Logging info
        /// </summary>
        /// <param name="message">Message for logging</param>
        /// <param name="userId">User Id (optional)</param>
        /// <param name="methodName">Name caller method (optional)</param>
        public void LogInfo(string message, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.info < _level)
                return;
            
            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.info, message, location, userId);
            SaveToDatabase("Information", message, location, userId: userId);
        }

        /// <summary>
        /// Logging debug
        /// </summary>
        /// <param name="message">Debug message for logging</param>
        /// <param name="userId">User Id (optional)</param>
        /// <param name="methodName">Name caller method (optional)</param>
        public void LogDebug(string message, string userId = null, [CallerMemberName] string methodName = "")
        {
            if (LoggerLevel.dbug < _level)
                return;

            var location = $"{_className} -> {methodName}";
            
            PrintLog(LoggerLevel.dbug, message, location, userId);
            SaveToDatabase("Debug", message, location, userId: userId);
        }

        // Writing log to Console
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

        // Saving log to database
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