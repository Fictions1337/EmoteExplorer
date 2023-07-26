using System;
using SpiderEye.Mac;
using EmoteExplorer.Core;

namespace EmoteExplorer
{
    class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            MacApplication.Init();
            Run();
        }
    }
}
