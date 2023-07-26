using SpiderEye;

namespace EmoteExplorer.Core
{
    public abstract class ProgramBase
    {
        protected static void Run()
        {
            using (var window = new Window())
            {
                window.Title = "EmoteExplorer";
                window.Icon = AppIcon.FromFile("icon", ".");
                window.EnableDevTools = false;
                window.EnableScriptInterface = true;
                

                var handler = new Handler();
                Application.AddGlobalHandler(handler);

                Application.ContentProvider = new EmbeddedContentProvider("App");
                Application.Run(window, "index.html");
            }
        }
    }
}
