using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
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

        internal static void SendAlertWebhookDiscord(TrackedProduct tracker)
        {
            try
            {
                var msg = $"Tracker alert for the following page:{Environment.NewLine}{tracker.PageURL}";
                if (!string.IsNullOrWhiteSpace(tracker.MentionString))
                {
                    msg = $"{tracker.MentionString} {msg}";
                }
                string jsonSend = JsonConvert.SerializeObject(new
                {
                    username = "ArbitraryBot",
                    avatar_url = Constants.WebHookAvatarURL,
                    embeds = new[]
                        {
                        new
                        {
                            description = msg,
                            title = "Keyword alert on the following tracker, Go Go Go!",
                            color = ""
                        }
                    }
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
    }
}
