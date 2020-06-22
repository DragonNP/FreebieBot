using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using CsQuery;
using FreebieBot.Models.Logger;

namespace FreebieBot.Models
{
    public class Parser
    {
        private readonly EventLogger _eventLogger;
        
         public Parser(EventLogger eventLogger)
        {
            _eventLogger = eventLogger;
        }
        
        public PostPikabu ParsePikabu()
        {
            string urlAddress = "https://pikabu.ru/community/steam/new";
            string data = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            
            _eventLogger.LogInfo("Parsing site Pikabu");
            var cq = CQ.Create(data);

            var storyId = Convert.ToInt64(cq.Find(".story")[0].GetAttribute("data-story-id"));
            var a = cq.Find(".story a")[0];
            var url = a.GetAttribute("href");
            var name = HttpUtility.HtmlDecode(a.InnerText);
            
            _eventLogger.LogDebug(storyId == default ? "Not found first story id" : $"Story ID: {storyId}");
            _eventLogger.LogDebug(string.IsNullOrEmpty(url) ? "Not found first url" : $"URL: {url}");
            _eventLogger.LogDebug(string.IsNullOrEmpty(url) ? "Not found name" : $"Name: {name}");
            
            var post = new PostPikabu()
            {
                Id = storyId,
                Name = name,
                Url = url
            };
            
            return post;
        }
    }
}