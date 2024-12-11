using System.IO;
using System.Xml.Serialization;

using As.Applications.IO;
using As.Applications.Loggers;
using As.Applications.Validation;

using ILogger = As.Applications.Loggers.ILogger;
using LogManager = Caliburn.Micro.LogManager;

namespace As.Applications.Config
{
    public class Settings : SettingsBase
    {
        static readonly ILogger Log = (ILogger)LogManager.GetLog(typeof(Settings));

        static Settings()
        {
            StoreOnExit = false;

            AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? APPLICATION_NAME;
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Log.InfoFormat($"Settings: {AppName} v{AppVersion}");

            StoreOnExit = XmlStream.Read(AppConfigPath, out App, create_if_missing: true, noexcept: true);
            App ??= new Settings();

            App.LoadMachineSettings();
            App.LoadGCodeSettings();
            System.Windows.Application.Current.Exit += Current_Exit;
        }

        #region Static properties
        public const string APPLICATION_NAME = "LaserGrayscales";

        const string EXTENTION = ".conf";

        static public readonly string AppName;

        /// <summary>
        /// VERSION as set in the project file, node Project.PropertyGroup.Version
        /// </summary>
        /// <remarks>
        /// Application version, format is 'major.minor.build.bugfix'
        /// The major number is incremented on new functiontionality or large changeover.
        /// The minor number is incremented (in steps of 2) on breaking changes, even numbers are release versions, odd numbers are debug versions.
        /// The build number is incremented on changes not adding new functionality(e.g.on optimising performance.)
        /// The bugfix number is incremented on bugfixes.
        /// </remarks>
        static public readonly Version? AppVersion;

        static string AppConfigPath => Path.Combine(".", $"{AppName}{EXTENTION}");

        static public readonly Settings App;

        static public bool StoreOnExit { get; set; }
        #endregion Static properties

        #region Static actions
        static void Current_Exit(object _, System.Windows.ExitEventArgs __)
        {
            // moving to exit, try not to throw exceptions, unknown why moving to exit.
            try
            {
                if (StoreOnExit)
                {
                    if (App.IsChanged) XmlStream.Write(AppConfigPath, App, create_backup: true, noexcept: true);
                    if (App.Machine.IsChanged) App.StoreMachineSettings(noexcept: true);
                    if (App.GCode.IsChanged) App.StoreGCodeSettings(noexcept: true);
                }
                Log.InfoFormat($"Config: Release {AppName} v{AppVersion}");
            }
            catch (Exception x)
            {
                try { Log.ErrorFormat("Current_Exit: exception", x); } catch { /* never mind */ }
            }
        }
        #endregion Static actions

        protected override void Initialise()
        {
            SetValue(
                nameof(Verbose),
                "false",
                "True: more information on screen and in test script",
                ValidString.ValidateIsBooleanValue);

            SetValue(
                nameof(Debug),
                "false",
                "True: debug mode, specific information for debugging.",
                ValidString.ValidateIsBooleanValue);

            SetValue(
                "BuildArea",
                @".\build",
                "Location for temporary files while building a test script",
                ValidPath.ValidatePath);

            SetValue(
                "ConfigArea",
                @".\config",
                "Location for configuration files",
                ValidPath.ValidatePath);

            SetValue(
                "MachineSettingsFileName",
                "machinesettings.xml",
                "Name of machine configuration file",
                ValidPath.ValidateFileName);

            SetValue(
                "GCodeSettingsFileName",
                "gcodesettings.xml",
                "Name of gcode configuration file",
                ValidPath.ValidateFileName);

            SetValue(
                "AutoLoadTestpatternPath",
                @".\TestPattern.xml",
                "Default test to load (if available)",
                ValidPath.ValidatePathOrEmpty);

            SetValue(
                nameof(TestscriptPath),
                "",
                "Default location for test scripts (default if empty: \"MyDocuments\")",
                ValidPath.ValidatePathOrEmpty);
        }

        #region Properties
        [XmlIgnore]
        public bool Verbose => Get<bool>();

        [XmlIgnore]
        public bool Debug => Get<bool>();

        [XmlIgnore]
        public string BuildArea => GetString() ?? "";

        [XmlIgnore]
        public string ConfigArea => GetString() ?? "";

        [XmlIgnore]
        public string MachineSettingsFileName => GetString() ?? "";

        [XmlIgnore]
        public string MachineSettingsPath => Path.Combine(ConfigArea, MachineSettingsFileName);

        [XmlIgnore]
        public MachineSettings Machine { get; private set; } = new MachineSettings();

        [XmlIgnore]
        public string GCodeSettingsFileName => GetString() ?? "";

        [XmlIgnore]
        public string GCodeSettingsPath => Path.Combine(ConfigArea, GCodeSettingsFileName);

        [XmlIgnore]
        public GCodeSettings GCode { get; private set; } = new GCodeSettings(set_defaults: true);

        [XmlIgnore]
        public string TestSettingsFileName => GetString() ?? "";

        [XmlIgnore]
        public string TestSettingsPath => Path.Combine(ConfigArea, TestSettingsFileName);

        [XmlIgnore]
        public string AutoLoadTestpatternPath => GetString() ?? "";

        [XmlIgnore]
        public string TestscriptPath => GetString() ?? "";
        #endregion Properties

        #region Actions
        public bool LoadMachineSettings(bool noexcept = false)
        {
            if (!XmlStream.Read(MachineSettingsPath, out MachineSettings result, create_if_missing: true, noexcept)) return false;
            Machine = result;
            return true;
        }

        public bool StoreMachineSettings(bool noexcept = false)
            => XmlStream.Write(MachineSettingsPath, Machine, create_backup: true, noexcept);

        public bool LoadGCodeSettings(bool noexcept = false)
        {
            var p = GCodeSettingsPath;
            if (!File.Exists(p))
            {
                GCode = new GCodeSettings(set_defaults: true);
                return XmlStream.Write(p, GCode, create_backup: false, noexcept);
            }

            if (!XmlStream.Read(GCodeSettingsPath, out GCodeSettings result, create_if_missing: false, noexcept))
            {
                var msg = $"LoadGCodeSettings: failed; using default settings.";
                UI.WarnFormat(msg);
                Log.WarnFormat(msg);
                GCode = new GCodeSettings(set_defaults: true);
                return false;
            }
            GCode = result;
            return true;
        }

        public bool StoreGCodeSettings(bool noexcept = false)
            => XmlStream.Write(GCodeSettingsPath, GCode, create_backup: true, noexcept);
        #endregion  Actions
    }
}
