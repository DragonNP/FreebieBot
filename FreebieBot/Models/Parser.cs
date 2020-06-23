using System;
using System.Web;
using CsQuery;
using FreebieBot.Models.Posts;
using FreebieBot.Services;

namespace FreebieBot.Models
{
    public class Parser
    {
        private readonly EventLoggerService _eventLogger;
        private CQ _stories;

        public Parser(EventLoggerService eventLogger)
        {
            _eventLogger = eventLogger;
            _eventLogger.AddClass<Parser>();
        }

        private void ParsePikabu()
        {
            _eventLogger.LogInfo("Parsing site Pikabu");
            const string urlAddress = "https://pikabu.ru/community/steam/new";

            var cq = CQ.CreateFromUrl(urlAddress);

            _stories = cq.Find(".stories-feed__container");
        }

        /// <summary>
        /// Searching next post from Pikabu
        /// </summary>
        /// <returns>Post or null (posts ended)</returns>
        public Post NextPikabuPost()
        {
            if (_stories == default)
                ParsePikabu();

            var story = _stories.Select(".story");

            if (story[0].ClassName != "story") return null;

            var id = Convert.ToInt64(story[0].GetAttribute("data-story-id"));

            var aElement = story.Find(".story__title-link")[0];
            if (aElement == null) return null;
            var name = HttpUtility.HtmlDecode(aElement.InnerText);
            var url = aElement.GetAttribute("href");

            var dateTime = DateTime.Parse(story.Find(".story__datetime")[0].GetAttribute("datetime"));

            var post = new Post()
            {
                Id = id,
                Name = name,
                Url = url,
                Type = PostType.Pikabu,
                DateTime = dateTime
            };

            _eventLogger.LogDebug(id == default ? "Not found pikabu post" : $"Pikabu post Id: {id}");

            story[0].Remove();

            return post;
        }
    }
}