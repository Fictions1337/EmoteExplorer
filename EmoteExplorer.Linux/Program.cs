using System;
using SpiderEye.Linux;
using EmoteExplorer.Core;

namespace EmoteExplorer
{
    class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            LinuxApplication.Init();
            Run();
        }
    }
}
