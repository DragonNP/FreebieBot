﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Translates;
using FreebieBot.Models.Users;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = FreebieBot.Models.Users.User;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public class UnsubscribePikabuCommand : TextCommand
    {
        protected override Line Name { get; set; }

        public override async Task<bool> Contains(Message message, ApplicationContext context)
        {
            if (message.Type != MessageType.Text)
                return false;

            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return false;

            if (Name == null)
                Name = await context.Lines.FindAsync("unsubscribeFrom");
            return Name.Contains(message.Text.Replace("Pikabu", "{0}"));
        }

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationContext context, TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database
            
            // If user not registered
            if (user == null) return;
            
            var unsubscribedFrom = (await context.Lines.FindAsync("unsubscribedFrom"))
                .GetTranslate(user.Lang);
            
            user.SubPikabu = UserSub.No;
            context.Users.Update(user);
            
            await client.SendTextMessageAsync(user.TelegramId, string.Format(unsubscribedFrom, "Pikabu"),
                replyMarkup: markups.GetSettingsMarkup(user));
            await context.SaveChangesAsync();
        }
    }
}