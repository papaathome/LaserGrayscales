using System.ComponentModel;
using System.IO;

using Caliburn.Micro;
using As.Applications.Data;
using As.Applications.Loggers;
using As.Applications.Models;
using As.Applications.Procedures;
using As.Applications.Appenders;

namespace As.Applications.ViewModels
{
    internal class MainViewModel : Conductor<object>, IDataErrorInfo, IViewValidInfo
    {
        static readonly ILogger log = (ILogger)LogManager.GetLog(typeof(MainViewModel));

        public MainViewModel()
        {
            X = new ScaleViewModel(Config.AppConfig.GrayScale.X, SelectedGroupMode);
            X.OnIsValidViewChanged += OnViewValidChanged;

            Y = new ScaleViewModel(Config.AppConfig.GrayScale.Y, SelectedGroupMode.Covariant());
            Y.OnIsValidViewChanged += OnViewValidChanged;

            ValidateGroupCount();
            ValidateGroupGap();
            ValidateGroupMode();

            UI.OnUiEventHandler += OnUiEvent;
        }

        #region IDataErrorInfo
        public string Error => string.Empty;

        public string this[string item]
            => item switch
            {
                nameof(GroupCount) => ValidateGroupCount(),
                nameof(GroupGap) => ValidateGroupGap(),
                nameof(GroupMode) => ValidateGroupMode(),
                _ => string.Empty,
            };
        #endregion IDataErrorInfo

        #region ViewValid
        bool _isvalid_view = false;
        public bool IsValidView
        {
            get { return _isvalid_view; }
            private set
            {
                if (_isvalid_view != value)
                {
                    _isvalid_view = value;
                    CanGenerate = value;
                    OnIsValidViewChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event IsValidViewChanged? OnIsValidViewChanged;

        void ValidateView()
        {
            IsValidView =
                X.IsValidView &&
                Y.IsValidView &&
                IsValidGroupCount &&
                IsValidGroupGap &&
                IsValidMode;
        }
        #endregion ViewValid

        #region Data
        public ScaleViewModel X { get; private set; }

        public ScaleViewModel Y { get; private set; }

        void OnViewValidChanged(object sender, EventArgs e)
            => ValidateView();

        public int GroupCount
        {
            get => Config.AppConfig.GrayScale.GroupCount;
            set
            {
                if (GroupCount != value)
                {
                    Config.AppConfig.GrayScale.GroupCount = value;
                    NotifyOfPropertyChange(nameof(GroupCount));
                }
            }
        }

        bool _isvalid_group_count = false;
        bool IsValidGroupCount
        {
            get { return _isvalid_group_count; }
            set
            {
                if (_isvalid_group_count != value)
                {
                    _isvalid_group_count = value;
                    ValidateView();
                }
            }
        }

        string ValidateGroupCount()
        {
            var result = string.Empty;
            IsValidGroupCount = GroupCount.TryIsValidMinimum(0, ref result, open_interval: true);
            return result;
        }

        public double GroupGap
        {
            get => Config.AppConfig.GrayScale.GroupGap;
            set
            {
                if (GroupGap != value)
                {
                    Config.AppConfig.GrayScale.GroupGap = value;
                    NotifyOfPropertyChange(nameof(GroupGap));
                }
            }
        }

        bool _isvalid_group_gap = false;
        bool IsValidGroupGap
        {
            get { return _isvalid_group_gap; }
            set
            {
                if (_isvalid_group_gap != value)
                {
                    _isvalid_group_gap = value;
                    ValidateView();
                }
            }
        }

        string ValidateGroupGap()
        {
            var result = string.Empty;
            IsValidGroupGap = GroupGap.TryIsValidMinimum(0, ref result);
            return result;
        }

        // see: https://stackoverflow.com/questions/9608282/how-to-do-caliburn-micro-binding-of-view-model-to-combobox-selected-value
        public List<Mode> GroupMode { get; private set; } = [Mode.Power, Mode.Speed];

        public Mode SelectedGroupMode
        {
            get => Config.AppConfig.GrayScale.Mode;
            set
            {
                if (SelectedGroupMode != value)
                {
                    Config.AppConfig.GrayScale.Mode = value;
                    NotifyOfPropertyChange(nameof(SelectedGroupMode));
                    SetScaleModes();
                }
            }
        }

        bool _isvalid_mode = false;
        bool IsValidMode
        {
            get { return _isvalid_mode; }
            set
            {
                if (_isvalid_mode != value)
                {
                    _isvalid_mode = value;
                    ValidateView();
                }
            }
        }

        private string ValidateGroupMode()
        {
            var result = string.Empty;
            switch (SelectedGroupMode)
            {
                case Mode.Power:
                case Mode.Speed:
                    IsValidMode = true;
                    break;
                default:
                    IsValidMode = false;
                    result = $"Group mode {SelectedGroupMode} not recognised";
                    break;
            }
            return result;
        }

        public string Intro
        {
            get => Config.AppConfig.GrayScale.Intro;
            set
            {
                if (Intro != value)
                {
                    Config.AppConfig.GrayScale.Intro = value;
                    NotifyOfPropertyChange(nameof(Intro));
                }
            }
        }

        public string Header
        {
            get => Config.AppConfig.GrayScale.Header;
            set
            {
                if (Header != value)
                {
                    Config.AppConfig.GrayScale.Header = value;
                    NotifyOfPropertyChange(nameof(Header));
                }
            }
        }

        public string Footer
        {
            get => Config.AppConfig.GrayScale.Footer;
            set
            {
                if (Footer != value)
                {
                    Config.AppConfig.GrayScale.Footer = value;
                    NotifyOfPropertyChange(nameof(Footer));
                }
            }
        }

        public string Testfile
        {
            get => Config.AppConfig.Testscript;
            set
            {
                if (Config.AppConfig.Testscript != value)
                {
                    Config.AppConfig.Testscript = value;
                    NotifyOfPropertyChange(nameof(Testfile));
                }
            }
        }
        #endregion Data

        public void TestFilePath()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = Testfile,
                DefaultExt = Path.GetExtension(Testfile),
                Filter = "GCode files |*.nc;*.ngc;*.gcode|all|*.*"
            };

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                Testfile = dlg.FileName;
            }
        }

        bool _can_generate = false;
        public bool CanGenerate
        {
            get { return _can_generate; }
            private set
            {
                if (_can_generate != value)
                {
                    _can_generate = value;
                    NotifyOfPropertyChange(nameof(CanGenerate));
                }
            }
        }

        public void Generate()
        {
            UI.InfoFormat($"Generate: path = \"{Config.AppConfig.Testscript}\"");
            GenerateValid = Manager.Generate();
        }

        bool _generate_valid = false;
        public bool GenerateValid
        {
            get { return _generate_valid; }
            private set
            {
                if (_generate_valid != value)
                {
                    _generate_valid = value;
                    NotifyOfPropertyChange(nameof(GenerateValid));
                }
            }
        }

        void SetScaleModes()
        {
            X.Mode = SelectedGroupMode;
            Y.Mode = SelectedGroupMode.Covariant();
        }

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

        void CondoleClear()
            => Console = string.Empty;
    }
}
