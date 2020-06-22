using System;

namespace FreebieBot.Models.Database
{
    public class Post
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public PostType Type { get; set; }
    }
}