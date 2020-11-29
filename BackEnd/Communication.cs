using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.Shared;
using Newtonsoft.Json;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Communication
    {
        internal static void SendAlertEmail(TrackedProduct tracker)
        {
            throw new NotImplementedException();
        }

        internal static void SendAlertWebhookDiscord(TrackedProduct tracker, string _title = null, string _msg = null, string _color = "718317")
        {
            try
            {
                if (_title == null)
                {
                    _title = $"Keyword alert {tracker.FriendlyName}, Go Go Go!";
                }
                if (_msg == null)
                {
                    _msg = $"Alerting on the tracker for the following page:{Environment.NewLine}{tracker.PageURL}";
                }
                string jsonSend = JsonConvert.SerializeObject(new
                {
                    username = "ArbitraryBot",
                    avatar_url = Constants.WebHookAvatarURL,
                    embeds = new[]
                        {
                        new
                        {
                            description = _msg,
                            title = _title,
                            color = _color
                        }
                    },
                    content = $"<@&{tracker.MentionString}>"
                });

                Log.Debug("Attempting to send webhook", tracker, jsonSend);
                WebRequest request = (HttpWebRequest)WebRequest.Create(tracker.WebHookURL);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(jsonSend);
                }
                Log.Debug("Sent webhook post", tracker, jsonSend);

                var response = (HttpWebResponse)request.GetResponse();
                Log.Information("Posted webhook", tracker, jsonSend, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failure occured when attempting to send tracker alert webhook", tracker);
            }
        }

        internal static async Task<WebCheck> DoesKeywordExistOnWebpage(string pageURL, string keyword)
        {
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var uri = new Uri(pageURL);

                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("Cache-Control", "max-age=0");
                    request.Headers.Add("DNT", "1");
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
                    request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    request.Headers.Add("Sec-Fetch-Site", "none");
                    request.Headers.Add("Sec-Fetch-Mode", "navigate");
                    request.Headers.Add("Sec-Fetch-User", "?1");
                    request.Headers.Add("Sec-Fetch-Dest", "document");
                    request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                    request.Headers.Add("Accept-Language", "en-US,en;q=0.9");

                    using (var response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        var contents = await response.Content.ReadAsStringAsync();
                        bool keywordFound = contents.Contains(keyword);
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        using (var streamReader = new StreamReader(decompressedStream))
                        {
                            var stream = await streamReader.ReadToEndAsync();
                            if (!keywordFound)
                            {
                                keywordFound = stream.Contains(keyword);
                            }
                            return new WebCheck()
                            {
                                KeywordExists = keywordFound,
                                ResponseCode = response.StatusCode,
                                WebpageContents = contents,
                                DecompressedContents = stream
                            };
                        }
                    }
                }
            }
        }

        internal static void ForceCanonicalPathAndQuery(Uri uri)
        {
            string paq = uri.PathAndQuery; // need to access PathAndQuery
            FieldInfo flagsFieldInfo = typeof(Uri).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
            ulong flags = (ulong)flagsFieldInfo.GetValue(uri);
            flags &= ~((ulong)0x30); // Flags.PathNotCanonical|Flags.QueryNotCanonical
            flagsFieldInfo.SetValue(uri, flags);
        }
    }
}
