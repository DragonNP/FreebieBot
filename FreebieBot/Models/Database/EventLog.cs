using System;
using System.ComponentModel.DataAnnotations;

namespace FreebieBot.Models.Database
{
    public class EventLog
    {
        [Key]
        public long EventId { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }
        public string StackTrace { get; set; }
        public string UserID { get; set; }
        public DateTime DateTime { get; set; }
    }
}