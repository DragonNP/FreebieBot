using System;

namespace FreebieBot.Models.Posts
{
    public class PostPikabu
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}