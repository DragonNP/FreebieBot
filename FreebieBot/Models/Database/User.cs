namespace FreebieBot.Models.Database
{
    public class User
    {
        public long Id { get; set; }
        public long TelegramId { get; set; }
        public string Name { get; set; }
        public UserLang Lang { get; set; }
        public UserSub SubPikabu { get; set; }
        public UserRole Role { get; set; }
    }
}