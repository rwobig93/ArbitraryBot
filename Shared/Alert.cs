using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbitraryBot.Shared
{
    public class Alert
    {
        public Guid AlertID { get; set; } = Guid.NewGuid();
        public AlertType AlertType { get; set; }
        public List<string> Emails { get; set; }
        public string WebHookURL { get; set; }
        public string MentionString { get; set; }
    }
}
