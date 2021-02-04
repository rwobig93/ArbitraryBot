using System.Net;

namespace ArbitraryBot.Dto
{
    public class WebCheck
    {
        public bool KeywordExists { get; set; }
        public bool WasCompressed { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public string WebpageContents { get; set; }
        public string DecompressedContents { get; set; }
    }
}
