using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreebieBot.Models;
using FreebieBot.Models.Database;
using FreebieBot.Models.Logger;
using FreebieBot.Models.TelegramBot;
using Microsoft.Extensions.Hosting;

namespace FreebieBot.Services
{
    public class FreebieHostedService : IHostedService, IDisposable
    {
        private readonly EventLogger _logger;
        private readonly ApplicationContext _context;
        private Timer _timer;

        public FreebieHostedService(EventLogger logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
            
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
            
            var postPikabu = parser.ParsePikabu();
            // TODO: Add more parser url

            var isNewPostPikabu = !_context.Posts.Any(p => p.Type == PostType.Pikabu && p.Id == postPikabu.Id);
            if (isNewPostPikabu)
            {
                var pattern = _context.Lines.Find("newFreebie");
                var text = new Line()
                {
                    Default = string.Format(pattern.Default.Replace("\\n", "\n"), PostType.Pikabu, postPikabu.Name, postPikabu.Url),
                    LineRus = string.Format(pattern.LineRus.Replace("\\n", "\n"), PostType.Pikabu, postPikabu.Name, postPikabu.Url)
                };
                var users = _context.Users.Where(p => p.SubPikabu == UserSub.Yes);
                SendTelegramMessage.Sends(users, text);

                _context.Posts.Add(new Post()
                {
                    Id = postPikabu.Id,
                    Name = postPikabu.Name,
                    DateTime = DateTime.Now
                });
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