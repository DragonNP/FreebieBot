﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FreebieBot.Models.Database;
using FreebieBot.Models.Translates;
using FreebieBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FreebieBot.Models.TelegramBot.TextCommands
{
    public class HowToUse : TextCommand
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
                Name = await context.Lines.FindAsync("howToUse");
            return Name.Contains(message.Text);
        }

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationContext context,
            TelegramMarkupsService markups)
        {
            var chat = message.Chat;
            var user = context.Users.FirstOrDefault(usr => usr.TelegramId == chat.Id); // Finding user in database

            // If user not registered
            if (user == null) return;

            var instructionLine = await context.Lines.FindAsync("instruction");
            var instruction = instructionLine.GetTranslate(user.Lang);
            await client.SendTextMessageAsync(user.TelegramId, instruction, replyMarkup: markups.GetMainMarkup(user.Lang));
        }
    }
}