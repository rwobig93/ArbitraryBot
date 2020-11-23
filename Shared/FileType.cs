using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbitraryBot.BackEnd;

namespace ArbitraryBot.Shared
{
    public class FileType
    {
        public AppFile AppFile { get; set; }
        public string FilePrefix { get; set; }
        public string Directory { get; set; }

        public static FileType GetFileType(AppFile appFile)
        {
            switch (appFile)
            {
                case AppFile.Config:
                    return new FileType
                    {
                        AppFile = AppFile.Config,
                        FilePrefix = "config_",
                        Directory = Constants.PathConfigDefault
                    };
                case AppFile.Log:
                    return new FileType
                    {
                        AppFile = AppFile.Log,
                        FilePrefix = OSDynamic.GetProductAssembly().ProductName,
                        Directory = Constants.PathLogs
                    };
                default:
                    return new FileType
                    {
                        AppFile = AppFile.Config,
                        FilePrefix = "config_",
                        Directory = Constants.PathConfigDefault
                    };
            }
        }
    }
}
