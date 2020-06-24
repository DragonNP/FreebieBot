using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Posts;
using FreebieBot.Models.Translates;
using FreebieBot.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public class SendLastFreebiePikabu : TextCommand
    {
        protected override Line Name { get; set; }
        private Line NewFreebie { get; set; }
        public override async Task<bool> Contains(Message message, ApplicationContext context)
        {
            if (message.Type != MessageType.Text)
                return false;

            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return false;

            if (Name == null)
                Name = await context.Lines.FindAsync("sendLastFreebie");
            return Name.Contains(message.Text.Replace("Pikabu", "{0}"));
        }

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return;
            
            if (NewFreebie == null)
                NewFreebie = await context.Lines.FindAsync("newFreebie");
            var newFreebie = NewFreebie.GetTranslate(user.Lang);

            var post = await context.Posts
                .Where(pst => pst.Type == PostType.Pikabu).
                OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            newFreebie = string.Format(newFreebie, post.Type, post.Name, post.Url);

            await client.SendTextMessageAsync(user.TelegramId, newFreebie, replyMarkup: markups.GetSettingsMarkup(user));
        }
    }
}