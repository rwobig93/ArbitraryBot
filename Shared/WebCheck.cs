using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArbitraryBot.Shared
{
    public class WebCheck
    {
        public bool KeywordExists { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public string WebpageContents { get; set; }
        public string DecompressedContents { get; set; }
    }
}
