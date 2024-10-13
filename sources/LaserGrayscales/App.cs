using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using As.Applications.Loggers;

namespace As.Applications
{
    public enum ApplicationExitCode
    {
        Success = 0,
        Failure = 1,
        CantWriteToApplicationLog = 2,
        CantPersistApplicationState = 3
    }

    public delegate void POnAppExit(object sender, ExitEventArgs e);

    public partial class App : Application
    {
        static internal ILogger? Log = null;

        static public event POnAppExit? OnAppExit;

        void App_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                // Write entry to application log, do we still have an application log?
                //Log?.Info($"Application exit = {e.ApplicationExitCode}");
            }
            catch
            {
                // Update exit code to reflect failure to write to application log
                e.ApplicationExitCode = (int)ApplicationExitCode.CantWriteToApplicationLog;
            }

            try
            {
                OnAppExit?.Invoke(sender, e);
            }
            catch
            {
                // Update exit code to reflect failure to persist application state
                e.ApplicationExitCode = (int)ApplicationExitCode.CantPersistApplicationState;
            }
        }
    }
}
