using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using As.Applications.Appenders;
using As.Applications.Config;
using As.Applications.Data;
using As.Applications.Data.Patterns;
using As.Applications.IO;
using As.Applications.Loggers;
using As.Applications.Plotters.Engine;
using As.Applications.Validation;

using Caliburn.Micro;

using ILogger = As.Applications.Loggers.ILogger;
using LogManager = Caliburn.Micro.LogManager;

namespace As.Applications.ViewModels
{
    internal class GrayscalesViewModel : Conductor<object>, IDataErrorInfo
    {
        static readonly ILogger Log = (ILogger)LogManager.GetLog(typeof(GrayscalesViewModel));

        // TODO: validate all procedurs.
        // TODO: generate preview

        public GrayscalesViewModel()
        {
            _dataErrorInfo = new(this);

            Image = new ImageViewModel();
            InnerPattern = new PatternViewModel() { Name = "Inner pattern" };
            OuterPattern = new PatternViewModel() { Name = "Outer pattern" };
            XScale = new ScaleViewModel() { Name = "X-axis" };
            YScale = new ScaleViewModel() { Name = "Y-axis" };

            GCode = new GCodeViewModel() { GCode = Settings.App.GCode, KV = Settings.App.GCode.Content };
            Machine = new MachineViewModel() { KV = Settings.App.Machine.Content };
            App = new AppViewModel() { KV = Settings.App.Content };

            UI.OnUiEventHandler += OnUiEvent;

            PropertyChanged += OnPropertyChanged;
            Image.PropertyChanged += OnPropertyChanged;
            InnerPattern.PropertyChanged += OnPropertyChanged;
            OuterPattern.PropertyChanged += OnPropertyChanged;
            XScale.PropertyChanged += OnPropertyChanged;
            YScale.PropertyChanged += OnPropertyChanged;

            Load(Settings.App.AutoLoadTestpatternPath);
        }

        #region Properties
        public string Name
        {
            get => _name;
            set
            {
                if (!Set(ref _name, value, value.ValidateFileName())) return;
                NotifyOfPropertyChange(nameof(CanSave));
            }
        }
        string _name = "testpattern";

        TestPattern test_pattern
        {
            get => _test_pattern;
            set
            {
                if (_test_pattern.Equals(value)) return;
                _test_pattern = value;
                NotifyOfPropertyChange(nameof(KV));
            }
        }
        TestPattern _test_pattern = new();

        public ObservableCollection<KVPair> KV => test_pattern.Content;

        public ImageViewModel Image { get; private set; }

        public PatternViewModel InnerPattern { get; private set; }

        public PatternViewModel OuterPattern { get; private set; }

        public ScaleViewModel XScale { get; private set; }

        public ScaleViewModel YScale { get; private set; }

        public GCodeViewModel GCode { get; private set; }

        public MachineViewModel Machine { get; private set; }

        public AppViewModel App { get; private set; }
        #endregion Properties

        #region Actions
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Name):
                case nameof(CanLoad):
                case nameof(CanSave):
                case nameof(CanPreview):
                case nameof(CanGenerate):
                    return;
            }
            var result = Error == "";
            CanPreview = result;
            CanGenerate = result;
        }

        const string FILTER = $"Grayscale test setting files |*{XmlStream.EXTENSION}|all|*.*";

        TestPattern GetTestPattern()
        {
            // TODO: build with operators.
            var result = new TestPattern()
            {
                Image = Image.GetImage(),
                InnerPattern = InnerPattern.GetPattern(),
                OuterPattern = OuterPattern.GetPattern(),

                XLimits = new Axis()
                {
                    Power = XScale.GetPowerScale(),
                    Speed = XScale.GetSpeedScale(),
                    ZHeight = XScale.GetHeightScale()
                },
                YLimits = new Axis()
                {
                    Power = YScale.GetPowerScale(),
                    Speed = YScale.GetSpeedScale(),
                    ZHeight = YScale.GetHeightScale()
                }
            };
            result.ImportValues(test_pattern);
            return result;
        }

        void SetTestPattern(string name, TestPattern value)
        {
            test_pattern = value;

            Name = name;
            Image.SetImage(value.Image);
            InnerPattern.SetPattern(value.InnerPattern);
            OuterPattern.SetPattern(value.OuterPattern);

            XScale.SetPowerScale(value.XLimits.Power);
            XScale.SetSpeedScale(value.XLimits.Speed);
            XScale.SetHeightScale(value.XLimits.ZHeight);

            YScale.SetPowerScale(value.YLimits.Power);
            YScale.SetSpeedScale(value.YLimits.Speed);
            YScale.SetHeightScale(value.YLimits.ZHeight);
        }

        public void Load()
        {
            var root = Settings.App.TestscriptPath;
            if (string.IsNullOrWhiteSpace(root)) root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var name = Name + XmlStream.EXTENSION;

            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                DefaultDirectory = root,
                InitialDirectory = root,
                FileName = name,
                DefaultExt = XmlStream.EXTENSION,
                Filter = FILTER
            };

            if (dlg.ShowDialog() ?? false) Load(dlg.FileName);
        }

        public bool CanLoad { get; private set; } = true;

        void Load(string path)
        {
            if (XmlStream.Read(path, out TestPattern result, create_if_missing: false, noexcept: false))
            {
                SetTestPattern(Path.GetFileNameWithoutExtension(path), result);
            }
        }

        public void Save()
        {
            var root = Settings.App.TestscriptPath;
            if (string.IsNullOrWhiteSpace(root)) root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var name = Name + XmlStream.EXTENSION;

            Microsoft.Win32.SaveFileDialog dlg = new()
            {
                DefaultDirectory = root,
                InitialDirectory = root,
                FileName = name,
                DefaultExt = XmlStream.EXTENSION,
                Filter = FILTER
            };

            if (dlg.ShowDialog() ?? false)
            {
                var testpattern = GetTestPattern();
                XmlStream.Write(dlg.FileName, testpattern, create_backup: true, noexcept: false);
            }
        }

        public bool CanSave => string.IsNullOrEmpty(this[nameof(Name)]);

        public void Preview()
        {
            // may ask file
            // generate bitmap image
        }

        public bool CanPreview
        {
            get => _can_preview;
            private set { /* ignore for the moment. */ } // => Set(ref _can_preview, value);
        }
        bool _can_preview = false;

        public void Generate()
        {
            var start = DateTime.Now;
            try
            {
                CanGenerate = false;
                //UI.InfoFormat("[[Generate]]");

                var p = new Plotter()
                {
                    Name = Name,
                    TestPattern = GetTestPattern(),
                    BuildPath = Settings.App.BuildArea
                };
                if (!p.TryGenerateGCode()) UI.InfoFormat("Generate: Failed to generate a GCode file, see log for details.");
                else
                {
                    if (Settings.App.Verbose) UI.InfoFormat("Generate: OK.");

                    var root = Settings.App.TestscriptPath;
                    if (string.IsNullOrWhiteSpace(root)) root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var name = Path.GetFileName(p.FilePath);
                    if (Path.GetExtension(name) == "") name += Plotter.EXTENSION;

                    Microsoft.Win32.SaveFileDialog dlg = new()
                    {
                        DefaultDirectory = root,
                        InitialDirectory = root,
                        FileName = name,
                        DefaultExt = Plotter.EXTENSION,
                        Filter = Plotter.FILTER
                    };
                    if (dlg.ShowDialog() ?? false)
                    {
                        try
                        {
                            File.Copy(p.FilePath, dlg.FileName);
                        }
                        catch (IOException x)
                        {
                            Log.Error($"Generate: file copy IO exception; message = \"{x.Message.Trim()}\"; source = \"{p.FilePath}\"; destination = \"{dlg.FileName}\"");
                            UI.ErrorFormat($"Generate: failed copy to \"{dlg.FileName}\"");
                        }
                        catch (Exception x)
                        {
                            Log.Error($"Generate: file copy exception; source = \"{p.FilePath}\"; destination = \"{dlg.FileName}\"", x);
                            UI.ErrorFormat($"Generate: failed copy to \"{dlg.FileName}\"");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error("Generate: failing", x);
                UI.ErrorFormat($"Generate: {x.Message.Trim()}");
            }
            finally
            {
                var stop = DateTime.Now;
                var duration = stop - start;
                var msg = $"Generate: calculation duration={Math.Truncate(duration.TotalMinutes):00}:{duration.Seconds:00}";
                Log.InfoFormat(msg);
                if (Settings.App.Verbose) UI.InfoFormat(msg);

                //UI.InfoFormat("[Generate]");
                CanGenerate = true;
            }
        }

        public bool CanGenerate
        {
            get => _can_generate;
            private set => Set(ref _can_generate, value);
        }
        bool _can_generate = false;

        public void ButtonA() { }

        public void ButtonB() { }

        public void ButtonC() { }

        public void ButtonD() { }

        public void ButtonE() { }
        #endregion Actions.

        #region Console
        string _console = string.Empty;
        public string Console
        {
            get { return _console; }
            set
            {
                if (_console != value)
                {
                    _console = value;
                    NotifyOfPropertyChange(nameof(Console));
                }
            }
        }

        void OnUiEvent(object? sender, MessageLoggedEventArgs e)
            => ConsoleWriteln(e.RenderedLoggingEvent);

        void ConsoleWriteln(string message)
        {
            if (string.IsNullOrEmpty(Console))
            {
                Console = message;
            }
            else
            {
                Console += Environment.NewLine + message;
            }
        }

        public void ConsoleClear() => Console = string.Empty;
        #endregion Console

        #region IDataErrorInfo
        /// <inheritdoc/>
        readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        /// <summary>
        /// Property setter for value, error info and to fire PropertyChanged
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        internal bool Set<T>(
            ref T lvalue,
            T value,
            string? error = null,
            [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (!_dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion
    }
}
