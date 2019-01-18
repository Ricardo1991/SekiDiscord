﻿using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SekiDiscord.Commands
{
    internal class Youtube
    {
        public static async Task YoutubeSearch(CommandContext ctx)
        {
            string message;
            string query;
            YoutubeSearch youtubeSearch = new YoutubeSearch();
            YoutubeVideoInfo youtubeVideo = new YoutubeVideoInfo();

            if (string.IsNullOrWhiteSpace(Settings.Default.apikey)) return;

            try
            {
                query = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                query = string.Empty;
            }

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
                if (searchResult.id.kind.ToLower() == "youtube#video")
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
                        await ctx.Message.RespondAsync(message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error getting youtube link info: " + ex.Message);
                    }
                }
            }

            await ctx.Message.RespondAsync("No Results Found");
            return;
        }
    }

    internal static class YoutubeUseful
    {
        public static string GetYoutubeInfoFromID(string id)
        {
            YoutubeVideoInfo youtubeVideo = new YoutubeVideoInfo();

            string getString = "https://www.googleapis.com/youtube/v3/videos/" + "?key=" + Settings.Default.apikey + "&part=snippet,contentDetails,statistics" + "&id=" + id;

            var webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            webClient.Headers.Add("User-agent", Settings.Default.UserAgent);
            try
            {
                string jsonYoutube = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonYoutube, youtubeVideo);

                string title = WebUtility.HtmlDecode(youtubeVideo.items[0].snippet.title);
                string duration = ParseDuration(youtubeVideo.items[0].contentDetails.duration);

                return "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + duration + "]\x02";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetYoutubeIdFromURL(string url)
        {
            string id;

            if (url.ToLower().Contains("youtu.be") && !url.ToLower().Contains("&feature"))
            {
                id = Useful.GetBetween(url, "youtu.be/", "?");
            }
            else
            {
                if (url.ToLower().Contains("?v="))
                    id = Useful.GetBetween(url, "?v=", "&");
                else
                    id = Useful.GetBetween(url, "&v=", "&");
            }

            return id.Split(new char[] { ' ', '\\', '"' }, 2)[0];
        }

        public static string ParseDuration(string duration)//PT#H#M#S
        {
            string temp = "";
            int hours = 0, minutes = 0, seconds = 0;

            duration = duration.Replace("PT", string.Empty);

            for (int i = 0; i < duration.Length; i++)
            {
                if (duration[i] != 'H' && duration[i] != 'M' && duration[i] != 'S')
                {
                    temp = temp + duration[i];
                }
                else
                    switch (duration[i])
                    {
                        case 'H':
                            hours = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;

                        case 'M':
                            minutes = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;

                        case 'S':
                            seconds = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;
                    }
            }

            if (hours > 0)
                return hours.ToString() + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            else
                return minutes.ToString() + ":" + seconds.ToString("00");
        }
    }

    public class YoutubeSearch
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
            public System.Uri url { get; set; }
        }

        public class Medium
        {
            public System.Uri url { get; set; }
        }

        public class High
        {
            public System.Uri url { get; set; }
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
                public System.DateTime publishedAt { get; set; }
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
                    public System.Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Medium
                {
                    public System.Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class High
                {
                    public System.Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Standard
                {
                    public System.Uri url { get; set; }
                    public int width { get; set; }
                    public int height { get; set; }
                }

                public class Maxres
                {
                    public System.Uri url { get; set; }
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