using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreebieBot.Models;
using FreebieBot.Models.Database;
using FreebieBot.Models.Users;
using FreebieBot.Models.Posts;
using FreebieBot.Models.Translates;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace FreebieBot.Services
{
    public class FreebieHostedService : IHostedService, IDisposable
    {
        private readonly EventLoggerService _logger;
        private readonly ApplicationContext _context;
        private Timer _timer;
        private readonly TelegramBotClient _telegramBot;

        public FreebieHostedService(EventLoggerService logger, ApplicationContext context, TelegramBotService telegramBot)
        {
            _logger = logger;
            _context = context;
            _telegramBot = telegramBot.GetBotClient();
            
            // Initializations
            _logger.AddClass<FreebieHostedService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Timed Background Service is starting.");
            
            _timer = new Timer(SearchFreebies, null, TimeSpan.Zero, 
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private void SearchFreebies(object state)
        {
            _logger.LogDebug("Starting parsing");
            var parser = new Parser(_logger);
            
            var postPikabu = parser.NextPikabuPost();
            // TODO: Add more parser url

            var pikabuFreebies = new List<Line>();

            // If new freebie from pikabu is no, that continuing the cycle
            while (postPikabu != null && !_context.Posts.Any(p => p.Type == postPikabu.Type && p.Id == postPikabu.Id))
            {
                var pattern = _context.Lines.Find("newFreebie"); // Finding pattern for translate
                var line = new Line() // Creating translate
                {
                    Default = string.Format(pattern.Default.Replace("\\n", "\n"), PostType.Pikabu, postPikabu.Name,
                        postPikabu.Url),
                    LineRus = string.Format(pattern.LineRus.Replace("\\n", "\n"), PostType.Pikabu, postPikabu.Name,
                        postPikabu.Url)
                };
                
                pikabuFreebies.Add(line);
                
                // Adding new post to database
                _context.Posts.Add(new Post()
                {
                    Id = postPikabu.Id,
                    Name = postPikabu.Name,
                    Url = postPikabu.Url,
                    Type = PostType.Pikabu,
                    DateTime = postPikabu.DateTime
                });
                
                
                // Finding next post from Pikabu
                postPikabu = parser.NextPikabuPost();
            }

            if (pikabuFreebies.Count == 0) return;
            // Searching users with Pikabu subscription
            var users = _context.Users.Where(p => p.SubPikabu == UserSub.Yes);
            SendTelegramMessage(users, pikabuFreebies).Wait();
            
            _context.SaveChangesAsync(); // Saving updates to database
        }

        private async Task SendTelegramMessage(IQueryable<User> users, List<Line> lines)
        {
            foreach (var user in users)
            {
                foreach (var line in lines)
                {
                    await _telegramBot.SendTextMessageAsync(user.TelegramId, user.Lang == UserLang.def ? line.Default : line.LineRus);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Timed Background Service is stopping");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}