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

        internal static void SendAlertWebhookDiscord(TrackedProduct tracker, string _title = null, string _msg = null, string _color = "718317")
        {
            try
            {
                if (_title == null)
                {
                    _title = "Keyword alert on the following tracker, Go Go Go!";
                }
                if (_msg == null)
                {
                    _msg = $"Alerting on tracker for the following page:{Environment.NewLine}{tracker.PageURL}";
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
    }
}
