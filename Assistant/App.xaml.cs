using Serilog;
using System.Windows;

namespace Assistant
{
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"logs\log")
                .CreateLogger();
        }
    }
}
