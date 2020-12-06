using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ArbitraryBot.Extensions;
using ArbitraryBot.FrontEnd;
using ArbitraryBot.Shared;
using MailKit.Net.Smtp;
using MimeKit;
using Newtonsoft.Json;
using Serilog;

namespace ArbitraryBot.BackEnd
{
    public static class Communication
    {
        public static HttpClient HttpClient { get; private set; }

        public static void Initialize()
        {
            if (HttpClient == null)
            {
                HttpClient = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Log.Debug("Initialized HttpClient");
            }
            else
            {
                Log.Warning("HttpClient isn't null but we're attempting to initialize anyway");
            }
        }

        public static void Dispose()
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
            }
            else
            {
                Log.Warning("HttpClient was already null but we're attempting to dispose anyway");
            }
        }

        internal static void SendAlertEmail(TrackedProduct tracker, string title = null, string msg = null)
        {
            try
            {
                Log.Debug("Attempting to send email alert: [{Tracker}] {Emails}", tracker.FriendlyName, tracker.Emails);
                if (title == null)
                    title = $"Keyword alert {tracker.FriendlyName}, Go Go Go!";
                if (msg == null)
                    msg = $"Alerting on the tracker for the following page:{Environment.NewLine}{tracker.PageURL}";

                Communication.SendEmail(tracker, title, msg);
                Log.Information("Sent email alert: [{Tracker}] {Emails}", tracker.FriendlyName, tracker.Emails);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occured on email alert");
                Handler.NotifyError($"Error on Tracker Alert: [{tracker.FriendlyName}] {ex.Message}");
            }
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
                Handler.NotifyError(ex, "Alert");
            }
        }

        internal static async Task<WebCheck> WebCheckForKeyword(string pageURL, string keyword)
        {
            if (HttpClient == null)
            {
                Initialize();
            }
            var uri = new Uri(pageURL);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddHttpHeaders(request);

            using var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            Log.Verbose("Webpage Contents:   {WebpageContents}", contents);
            if (string.IsNullOrWhiteSpace(contents))
            {
                return null;
            }
            else if (contents.Contains("The browser you're using doesn't support JavaScript, or has JavaScript turned off."))
            {
                throw new Exception("Javascript required, bot blocking method in place");
            }
            bool keywordFound = contents.Contains(keyword);
            bool pageCompressed = contents.IsCompressed();
            if (!pageCompressed)
            {
                return new WebCheck()
                {
                    KeywordExists = keywordFound,
                    ResponseCode = response.StatusCode,
                    WebpageContents = contents,
                    WasCompressed = pageCompressed
                };
            }
            using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(decompressedStream);
            var stream = await streamReader.ReadToEndAsync();
            keywordFound = stream.Contains(keyword);
            Log.Verbose("Webpage Uncompressed: {WebpageUncompressed}", stream);
            return new WebCheck()
            {
                KeywordExists = keywordFound,
                ResponseCode = response.StatusCode,
                WebpageContents = contents,
                DecompressedContents = stream,
                WasCompressed = pageCompressed
            };
        }

        private static void AddHttpHeaders(HttpRequestMessage request)
        {
            Log.Verbose("Adding Http Headers");
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
            Log.Verbose("Finished Adding Http Headers");
        }

        internal static async Task<WebCheck> GetWebFileContentsUncompressed(string pageURL)
        {
            if (HttpClient == null)
            {
                Initialize();
            }
            var uri = new Uri(pageURL);

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            AddHttpHeaders(request);

            using var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            Log.Verbose("Webpage Content: {WebpageCompressed}", contents);
            return new WebCheck()
            {
                ResponseCode = response.StatusCode,
                WebpageContents = contents
            };
        }

        private static void SendEmail(TrackedProduct tracker, string title, string msg)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(Constants.Config.SMTPEmailName, Constants.Config.SMTPEmailFrom));
            foreach (var address in tracker.Emails)
            {
                mailMessage.Bcc.Add(new MailboxAddress(address.ToString()));
            }

            mailMessage.Subject = title;
            mailMessage.Body = new TextPart("plain")
            {
                Text = msg
            };

            using var smtpClient = new SmtpClient();
            smtpClient.Connect(Constants.Config.SMTPUrl, Constants.Config.SMTPPort,  MailKit.Security.SecureSocketOptions.Auto);
            smtpClient.Authenticate(Constants.Config.SMTPUsername, Constants.Config.SMTPPassword);
            smtpClient.Send(mailMessage);
            smtpClient.Disconnect(true);
        }
    }
}
