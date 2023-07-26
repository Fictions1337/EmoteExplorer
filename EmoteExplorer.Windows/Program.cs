using System;
using SpiderEye.Windows;
using EmoteExplorer.Core;

namespace EmoteExplorer
{
    class Program : ProgramBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            WindowsApplication.Init();
            Run();
        }
    }
}
