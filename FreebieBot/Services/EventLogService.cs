using System;
using FreebieBot.Models;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;

namespace FreebieBot.Services
{
    public class EventLogService
    {
        private static EventLogLevel _level = EventLogLevel.fail;
        private static DatabaseContext _db;

        public EventLogService(DatabaseContext db)
        {
            _db = db;
        }

        public static void SetLevel(string level)
        {
            switch (level)
            {
                case "debug":
                    _level = EventLogLevel.dbug;
                    break;
                case "info":
                    _level = EventLogLevel.info;
                    break;
                case "error":
                    _level = EventLogLevel.fail;
                    break;
                case "critical":
                    _level = EventLogLevel.crit;
                    break;
            }
        }

        /// <summary>
        /// Logs Event To DataBase.
        /// </summary>
        /// <param name="type">Type of event. Use "Error" or "Info".</param>
        /// <param name="message">Message about event.</param>
        /// <param name="location">Where this event happend.</param>
        /// <param name="exception">If have exception pass it. Can be nullable.</param>
        /// <param name="userId">Current user ID. Can be nullable.</param>
        public void LogEvent(string type, string message, string location, Exception exception = null,
            string userId = null)
        {
            _db.EventLogs.Add(new EventLog()
            {
                Type = type,
                Message = message,
                Location = location,
                StackTrace = exception?.ToString(),
                UserID = userId ?? null,
                DateTime = DateTime.Now
            });
            _db.SaveChanges();
        }

        public void LogCritical(Exception e, string location, string userId = null)
        {
            if (EventLogLevel.crit < _level)
                return;
            
            PrintLog(EventLogLevel.crit, e.Message, location, userId);
            SaveToDatabase("Critical", e.Message, location, e.StackTrace, userId);
        }

        public void LogError(Exception e, string location, string userId = null)
        {
            if (EventLogLevel.fail < _level)
                return;
            
            PrintLog(EventLogLevel.fail, e.Message, location, userId);
            SaveToDatabase("Error", e.Message, location, e.StackTrace, userId);
        }

        public void LogInfo(string message, string location, string userId = null)
        {
            if (EventLogLevel.info < _level)
                return;
            
            PrintLog(EventLogLevel.info, message, location, userId);
            SaveToDatabase("Information", message, location, userId: userId);
        }

        public void LogDebug(string message, string location, string userId = null)
        {
            if (EventLogLevel.dbug < _level)
                return;
            
            PrintLog(EventLogLevel.dbug, message, location, userId);
            SaveToDatabase("Debug", message, location, userId: userId);
        }

        private static void PrintLog(EventLogLevel level, string message, string location, string userId = null)
        {
            ConsoleColor foregroundColor, backgroundColor;
            switch (level)
            {
                case EventLogLevel.dbug:
                    foregroundColor = ConsoleColor.Gray;
                    backgroundColor = default;
                    break;
                case EventLogLevel.info:
                    foregroundColor = ConsoleColor.Green;
                    backgroundColor = default;
                    break;
                case EventLogLevel.fail:
                    foregroundColor = ConsoleColor.White;
                    backgroundColor = ConsoleColor.Red;
                    break;
                case EventLogLevel.crit:
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

        private static void SaveToDatabase(string level, string message, string location,
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