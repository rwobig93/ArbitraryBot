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
        public int RetentionDays { get; set; }

        public static FileType GetFileType(AppFile appFile)
        {
            switch (appFile)
            {
                case AppFile.Config:
                    return new FileType
                    {
                        AppFile = AppFile.Config,
                        FilePrefix = "Config_",
                        Directory = Constants.PathConfigDefault,
                        RetentionDays = 7
                    };
                case AppFile.Log:
                    return new FileType
                    {
                        AppFile = AppFile.Log,
                        FilePrefix = OSDynamic.GetProductAssembly().ProductName,
                        Directory = Constants.PathLogs,
                        RetentionDays = 30
                    };
                case AppFile.SavedData:
                    return new FileType
                    {
                        AppFile = AppFile.SavedData,
                        FilePrefix = "SaveData_",
                        Directory = Constants.PathSavedData,
                        RetentionDays = 90
                    };
                default:
                    return new FileType
                    {
                        AppFile = AppFile.Log,
                        FilePrefix = OSDynamic.GetProductAssembly().ProductName,
                        Directory = Constants.PathLogs,
                        RetentionDays = 30
                    };
            }
        }
    }
}
