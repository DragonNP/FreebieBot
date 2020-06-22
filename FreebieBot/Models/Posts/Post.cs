using System;

namespace FreebieBot.Models.Posts
{
    public class Post
    {
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public PostType Type { get; set; }
        
        public DateTime DateTime { get; set; }
    }
}