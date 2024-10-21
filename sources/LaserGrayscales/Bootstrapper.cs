using System.Windows;

using Caliburn.Micro;
using As.Applications.Loggers;
using As.Applications.ViewModels;
using As.Applications.Data;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = @".\LaserGrayscales.log4net.config", Watch = true)]

namespace As.Applications
{
    public class Bootstrapper : BootstrapperBase
    {
        static readonly ILogger log;

        static Bootstrapper()
        {
            LogManager.GetLog = type => new CmLog4NetLogger(type);
            log = (ILogger)LogManager.GetLog(typeof(Bootstrapper));
        }

        public Bootstrapper()
        {
            Initialize();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync(typeof(MainViewModel));
            UI.Debug($"Loaded: {Config.AppName}, v{Config.AppVersion}");
        }
    }
}
