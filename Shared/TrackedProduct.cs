using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbitraryBot.Shared
{
    public class TrackedProduct
    {
        public Guid TrackerID { get; } = new Guid();
        public string FriendlyName { get; set; }
        public string PageURL { get; set; }
        public string Keyword { get; set; }
        public bool AlertOnKeywordNotExist { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public TrackInterval AlertInterval { get; set; }
        public Alert AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
