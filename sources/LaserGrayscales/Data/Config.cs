using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

using Caliburn.Micro;
using As.Applications.Loggers;
using As.Applications.Models;
using UI = As.Applications.Loggers.UI;

namespace As.Applications.Data
{
    [Serializable]
    [XmlRoot(ElementName = "root")]
    public class Config : IEquatable<Config>
    {
        #region Class
        /// <summary>
        /// Application version, format is 'major.minor.build.bugfix'
        /// The major number is incremented on new functiontionality or large changeover.
        /// The minor number is incremented (in steps of 2) on breaking changes, even numbers are release versions, odd numbers are debug versions.
        /// The build number is incremented on changes not adding new functionality(e.g.on optimising performance.)
        /// The bugfix number is incremented on bugfixes.
        /// </summary>
        //public const string VERSION is set in the project file, node Project.PropertyGroup.Version

        /// <summary>
        /// Configuration file version, updated on changes in the configuration file.
        /// </summary>
        const string FILE_VERSION = "0.1.0";

        const string EXTENTION = ".conf";

        static readonly ILogger log = (ILogger)LogManager.GetLog(typeof(Config));

        /// <summary>
        /// Get Application name and version, load application configuration then set StoreOnExit true.
        /// </summary>
        static Config()
        {
            StoreOnExit = false;

            AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? Manager.APPLICATION_NAME;
            AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            log.InfoFormat($"Config: Load {AppName} v{AppVersion}");
            UI.InfoFormat($"Config: Load {AppName} v{AppVersion}");

            AppConfig = new Config(); // just to prevent compiler nagging.
            _app_config = new Config();

            StoreOnExit = LoadApp() != null;
            //Application.Current.Exit += Current_Exit;
        }

        /// <summary>
        /// Application is closing, check pending AppConfig changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Current_Exit(object sender, ExitEventArgs e)
        {
            if (StoreOnExit && AppConfigChanged) _StoreApp();
            UI.InfoFormat($"Config: Release {AppName} v{AppVersion}");
            log.InfoFormat($"Config: Release {AppName} v{AppVersion}");
        }

        /// <summary>
        /// Application name as retrieved from the executing assembly.
        /// </summary>
        static public readonly string AppName;

        /// <summary>
        /// Application version as retrieved from the executing assembly.
        /// </summary>
        static public readonly Version? AppVersion;

        /// <summary>
        /// True: store any pending AppConfig changes on exit; false: ignore any pending AppConfig changes on exit.
        /// </summary>
        static public bool StoreOnExit { get; set; }

        /// <summary>
        /// Application configuration, loaded on application start.
        /// </summary>
        static public Config AppConfig { get; set; }

        /// <summary>
        /// Used to check changes in AppConfig
        /// </summary>
        /// <remarks>For small ammounts of data it is easier to keep a copy to check changes than to track all actions on it.</remarks>
        static Config _app_config;

        /// <summary>
        /// Check if AppConfig is changed since last load/store action.
        /// </summary>
        static bool AppConfigChanged => !AppConfig.Equals(_app_config);

        /// <summary>
        /// Read application config from file
        /// </summary>
        /// <param name="path">(optional) Path to the file holding configuration data</param>
        /// <param name="create_file_if_missing">True: if no file then write one with default settings; false: if no file then use default settings but do not create a file</param>
        /// <returns>Application configuration if valid data was read; null otherwise</returns>
        static public Config LoadApp(string? path = null, bool create_file_if_missing = true)
        {
            AppConfig = _LoadApp(path, create_file_if_missing);
            _app_config = new Config(AppConfig);
            return AppConfig;
        }

        static Config _LoadApp(string? path = null, bool create_file_if_missing = true)
        {
            path ??= Path.Combine(".", $"{AppName}{EXTENTION}");
            var file = new FileInfo(path);
            Config? config = Load(file, noexcept:true);
            if (config != null)
            {
                log.DebugFormat($"Config.LoadApp: OK; path = \"{file.FullName}\"");
                return config;
            }

            config = new Config(defaults:true);
            if (create_file_if_missing)
            {
                log.WarnFormat($"Config.LoadApp: config file not found, create default settings; path = \"{file.FullName}\"");
                Store(file.FullName, config, noexcept: true);
            }
            else
            {
                log.WarnFormat($"Config.LoadApp: config file not found, using default settings; path = \"{file.FullName}\"");
            }
            return config;
        }

        /// <summary>
        /// Write applicaiton config to file
        /// </summary>
        /// <param name="path">(optional) Path to config file, will create or overwrite.</param>
        /// <returns>True if a valid config file is (re)written; false otherwise.</returns>
        static public bool StoreApp(string? path = null)
        {
            var result = _StoreApp(path);
            _app_config = new Config(AppConfig);
            return result;
        }

        static bool _StoreApp(string? path = null)
        {
            path ??= Path.Combine(".", $"{AppName}{EXTENTION}");
            var result = Store(path, AppConfig, noexcept: true);
            log.DebugFormat($"Config.StoreApp: {(result ? "OK" : "failed")}; path = \"{path}\"");
            return result;
        }

        /// <summary>
        /// Read config from file
        /// </summary>
        /// <param name="path">Path to the file holding configuration data</param>
        /// <param name="noexcept">True: do not throw exception and return null; false: let exceptions pass</param>
        /// <returns>Configuration if valid data was read; null otherwise</returns>
        static public Config? Load(string path, bool noexcept = false)
        {
            if (path == null) return null;
            return Load(new FileInfo(path), noexcept);
        }

        static Config? Load(FileInfo file, bool noexcept = false)
        {
            try
            {
                if ((file == null) || !file.Exists) return null;

                using Stream stream = new FileStream(file.FullName, FileMode.Open);
                return Load(stream);
            }
            catch (Exception x)
            {
                if (noexcept)
                {
                    log.ErrorFormat($"Config.Load: fail; path = \"{file.FullName}\"", x);
                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Read config from stream
        /// </summary>
        /// <param name="stream">stream holding configuration data</param>
        /// <param name="noexcept">True: do not throw exception and return null; false: let exceptions pass</param>
        /// <returns>Configuration if valid data was read; null otherwise</returns>
        static public Config? Load(Stream stream, bool noexcept = false)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Config));
                return serializer.Deserialize(stream) as Config;
            }
            catch (Exception x)
            {
                if (noexcept)
                {
                    log.ErrorFormat($"Config.Load: fail", x);
                    return null;
                }
                else throw;
            }
        }

        /// <summary>
        /// Write config to file
        /// </summary>
        /// <param name="path">Path to config file, will create or overwrite.</param>
        /// <param name="value">Configuration content</param>
        /// <returns>True if a valid config file is (re)written; false otherwise.</returns>
        static public bool Store(string path, Config? value, bool noexcept = false)
        {
            if (value == null) return false;
            return Store(new FileInfo(path), value, noexcept);
        }

        static bool Store(FileInfo file, Config value, bool noexcept = false)
        {
            try
            {
                using Stream stream = new FileStream(file.FullName, FileMode.Create);
                return Store(stream, value, noexcept: false);
            }
            catch (Exception x)
            {
                if (noexcept)
                {
                    log.ErrorFormat($"Config.Store: fail; path = \"{file.FullName}\"", x);
                    return false;
                }
                throw;
            }
        }

        static public bool Store(Stream stream, Config value, bool noexcept = false)
        {
            XmlWriter? writer = null;
            try
            {
                writer = XmlWriter.Create(stream, GetDefaultSettings());
                writer.WriteStartDocument();

                var serializer = new XmlSerializer(typeof(Config));
                serializer.Serialize(writer, value);

                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                return true;
            }
            catch (Exception x)
            {
                if (noexcept)
                {
                    log.ErrorFormat($"Config.Store: stream fail", x);
                    return false;
                }
                throw;
            }
            finally
            {
                writer?.Dispose();
            }
        }

        static XmlWriterSettings GetDefaultSettings()
        {
            var settings = new XmlWriterSettings
            {
                CheckCharacters = true,
                ConformanceLevel = ConformanceLevel.Document,
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                NewLineOnAttributes = false
            };
            return settings;
        }
        #endregion Class

        /// <summary>
        /// Create new config (with empty lists)
        /// </summary>
        /// <remarks>An Xml desirialiser will fill the lists by appending lines</remarks>
        public Config()
        {
            GrayScale = new GrayScale();
            //Clear();
        }

        /// <summary>
        /// Create new config (optional: with default settings in lists)
        /// </summary>
        /// <param name="defaults">true: fill lists with defaults; false keep lists empty</param>
        public Config(bool defaults)
        {
            GrayScale = new GrayScale(defaults);
            //if (!defaults) Clear();
        }

        Config(Config other)
        {
            Version = other.Version;
            PowerMinimum = other.PowerMinimum;
            PowerMaximum = other.PowerMaximum;
            SpeedMinimum = other.SpeedMinimum;
            SpeedMaximum = other.SpeedMaximum;
            GrayScale = new GrayScale(other.GrayScale);
            Testscript = other.Testscript;
        }

        //void Clear()
        //{
        //    // todo: clear any list with default content.
        //    // otherwise it will grow without end by adding default values on reading content.
        //
        //    // nothing to do.
        //}

        #region IEquatable
        public bool Equals(Config? other)
        {
            if (other == null) return false;
            return
                Version.Equals(other.Version) &&
                GrayScale.Equals(other.GrayScale) &&
                Testscript.Equals(other.Testscript) &&
                SpeedMinimum.Equals(other.SpeedMinimum) &&
                SpeedMaximum.Equals(other.SpeedMaximum) &&
                PowerMinimum.Equals(other.PowerMinimum) &&
                PowerMaximum.Equals(other.PowerMaximum);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not Config)) return false;
            return Equals(obj as Config);
        }

        public override int GetHashCode()
        {
            int result = 0x5a5a5a5a;
            result ^= Version.GetHashCode();
            result ^= PowerMinimum.GetHashCode();
            result ^= PowerMaximum.GetHashCode();
            result ^= SpeedMinimum.GetHashCode();
            result ^= SpeedMaximum.GetHashCode();
            result ^= GrayScale.GetHashCode();
            result ^= Testscript.GetHashCode();
            return result;
        }
        #endregion IEquatable

        #region Data
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; } = FILE_VERSION;

        [XmlElement(ElementName = "power_minimum")]
        public int PowerMinimum { get; set; } = 0;

        [XmlElement(ElementName = "power_maximum")]
        public int PowerMaximum { get; set; } = 255;

        [XmlElement(ElementName = "speed_minimum")]
        public int SpeedMinimum { get; set; } = 1;

        [XmlElement(ElementName = "speed_maximum")]
        public int SpeedMaximum { get; set; } = 70;

        [XmlElement(ElementName = "grayscale")]
        public GrayScale GrayScale { get; set; }

        [XmlElement(ElementName = "test_script")]
        public string Testscript { get; set; } = @".\LaserGrayscaleTest.nc";
        #endregion Data
    }
}
