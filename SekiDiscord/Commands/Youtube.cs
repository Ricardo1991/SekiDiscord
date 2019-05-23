using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;

namespace SekiDiscord.Commands
{
    internal class Youtube
    {
        public static string YoutubeSearch(string query)
        {
            string message;
            YoutubeSearch youtubeSearch = new YoutubeSearch();
            YoutubeVideoInfo youtubeVideo = new YoutubeVideoInfo();

            if (string.IsNullOrWhiteSpace(Settings.Default.apikey))
                throw new Exception("No API key for YoutubeSearch");

            string getString = "https://www.googleapis.com/youtube/v3/search" + "?key=" + Settings.Default.apikey + "&part=id,snippet" + "&q=" +
                HttpUtility.UrlEncode(query) + "&maxresults=10&type=video&safeSearch=none";

            var webClient = new WebClient()
            {
                Encoding = Encoding.UTF8
            };
            webClient.Headers.Add("User-Agent", Settings.Default.UserAgent);

            try
            {
                string jsonYoutube = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonYoutube, youtubeSearch);
            }
            catch { }

            foreach (var searchResult in youtubeSearch.items)
            {
                if (searchResult.id.kind.ToLower(CultureInfo.CreateSpecificCulture("en-GB")) == "youtube#video")
                {
                    try
                    {
                        getString = "https://www.googleapis.com/youtube/v3/videos/" + "?key=" + Settings.Default.apikey + "&part=snippet,contentDetails,statistics" +
                        "&id=" + searchResult.id.videoId;
                        string jsonYoutube = webClient.DownloadString(getString);
                        JsonConvert.PopulateObject(jsonYoutube, youtubeVideo);

                        string title = WebUtility.HtmlDecode(youtubeVideo.items[0].snippet.title);
                        string duration = YoutubeUseful.ParseDuration(youtubeVideo.items[0].contentDetails.duration);

                        message = "https://www.youtube.com/watch?v=" + searchResult.id.videoId;
                        return message;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error getting youtube link info: " + ex.Message);
                    }
                }
            }

            return "No Results Found";
        }
    }

    public static class YoutubeUseful
    {
        public static string ParseDuration(string duration)//PT#H#M#S
        {
            string temp = "";
            int hours = 0, minutes = 0, seconds = 0;

            duration = duration.Replace("PT", string.Empty, StringComparison.OrdinalIgnoreCase);

            for (int i = 0; i < duration.Length; i++)
            {
                if (duration[i] != 'H' && duration[i] != 'M' && duration[i] != 'S')
                {
                    temp += duration[i];
                }
                else
                    switch (duration[i])
                    {
                        case 'H':
                            hours = Convert.ToInt32(temp, CultureInfo.CreateSpecificCulture("en-GB"));
                            temp = string.Empty;
                            break;

                        case 'M':
                            minutes = Convert.ToInt32(temp, CultureInfo.CreateSpecificCulture("en-GB"));
                            temp = string.Empty;
                            break;

                        case 'S':
                            seconds = Convert.ToInt32(temp, CultureInfo.CreateSpecificCulture("en-GB"));
                            temp = string.Empty;
                            break;
                    }
            }

            if (hours > 0)
                return hours.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ":" + minutes.ToString("00", CultureInfo.CreateSpecificCulture("en-GB")) + ":" + seconds.ToString("00", CultureInfo.CreateSpecificCulture("en-GB"));
            else
                return minutes.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + ":" + seconds.ToString("00", CultureInfo.CreateSpecificCulture("en-GB"));
        }
    }

    internal class YoutubeSearch
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public Pageinfo pageInfo { get; set; }
        public Item[] items { get; set; }

        public class Pageinfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public Id id { get; set; }
            public Snippet snippet { get; set; }
        }

        public class Id
        {
            public string kind { get; set; }
            public string videoId { get; set; }
        }

        public class Snippet
        {
            public DateTime publishedAt { get; set; }
            public string channelId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public Thumbnails thumbnails { get; set; }
            public string channelTitle { get; set; }
            public string liveBroadcastContent { get; set; }
        }

        public class Thumbnails
        {
            public Default _default { get; set; }
            public Medium medium { get; set; }
            public High high { get; set; }
        }

        public class Default
        {
            public Uri url { get; set; }
        }

        public class Medium
        {
            public Uri url { get; set; }
        }

        public class High
        {
            public Uri url { get; set; }
        }
    }

    public class YoutubeVideoInfo
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Pageinfo pageInfo { get; set; }
        public Item[] items { get; set; }

        public class Pageinfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Snippet snippet { get; set; }
            public Contentdetails contentDetails { get; set; }
            public Statistics statistics { get; set; }

            public class Snippet
            {
                public DateTime publishedAt { get; set; }
                public string channelId { get; set; }
                public string title { get; set; }
                public string description { get; set; }
                public Thumbnails thumbnails { get; set; }
                public string channelTitle { get; set; }
                public string categoryId { get; set; }
                public string liveBroadcastContent { get; set; }
            }

            public class Thumbnails
            {
                public Default _default { get; set; }
                public Medium medium { get; set; }
                public High high { get; set; }
                public Standard standard { get; set; }
                public Maxres maxres { get; set; }

                public class Default
                {
                    public Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Medium
                {
                    public Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class High
                {
                    public Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Standard
                {
                    public Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Maxres
                {
                    public Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }
            }

            public class Contentdetails
            {
                public string duration { get; set; }
                public string dimension { get; set; }
                public string definition { get; set; }
                public string caption { get; set; }
                public bool licensedContent { get; set; }
            }

            public class Statistics
            {
                public string viewCount { get; set; }
                public string likeCount { get; set; }
                public string dislikeCount { get; set; }
                public string favoriteCount { get; set; }
                public string commentCount { get; set; }
            }
        }
    }
}