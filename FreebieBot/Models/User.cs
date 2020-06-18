namespace FreebieBot.Models
{
    public class User
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
        public bool IsSubPikabu { get; set; } = false;
    }
}