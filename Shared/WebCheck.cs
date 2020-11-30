using System.Net;

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
