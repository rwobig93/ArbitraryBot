using System;

namespace ArbitraryBot.Shared
{
    public class ProductAssembly
    {
        public string FilePath { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public string NewUpdateURL { get; set; }
        public bool NeedsUpdate { get; set; }
        public bool IsBeta { get; set; } = false;
        public Version AppVersion { get; set; }
        public Version LatestVersion { get; set; }
    }
}
