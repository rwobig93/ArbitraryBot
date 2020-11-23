using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbitraryBot.Shared
{
    public enum StatusReturn
    {
        NotFound,
        Found,
        Failure,
        Success
    }

    public enum AppFile
    {
        Config,
        Log
    }
}
