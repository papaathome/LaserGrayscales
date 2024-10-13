using System.ComponentModel;

using Caliburn.Micro;
using As.Applications.Data;
using As.Applications.Loggers;
using As.Applications.Models;

namespace As.Applications.ViewModels
{
    internal class MainViewModel : Conductor<object>, IDataErrorInfo
    {
        static readonly ILogger log = (ILogger)LogManager.GetLog(typeof(MainViewModel));

        public MainViewModel()
        {
            X = new ScaleViewModel(Config.AppConfig.GrayScale.X, SelectedGroupMode);
            Y = new ScaleViewModel(Config.AppConfig.GrayScale.Y, SelectedGroupMode.Covariant());
        }

        #region IDataErrorInfo
        public string Error => string.Empty;

        public string this[string item]
            => item switch
            {
                nameof(GroupCount) => ValidateGroupCount,
                nameof(GroupGap) => ValidateGroupGap,
                nameof(GroupMode) => ValidateGroupMode,
                _ => string.Empty,
            };
        #endregion IDataErrorInfo

        #region Data
        public ScaleViewModel X { get; private set; }

        public ScaleViewModel Y { get; private set; }

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

        private string ValidateGroupCount => (0 < GroupCount) ? string.Empty : $"Group count {GroupCount} less than 0";

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

        private string ValidateGroupGap => (0 < GroupGap) ? string.Empty : $"Group gap {GroupGap} less than 0";

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

        private string ValidateGroupMode
            => SelectedGroupMode switch
            {
                Mode.Speed => string.Empty,
                Mode.Power => string.Empty,
                _ => $"Group mode {SelectedGroupMode} not recognised",
            };

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

        // TODO v0.3: validating status must be OK.
        //public bool CanGenerate => true;

        public void Generate()
        {
            log.InfoFormat($"Generate: path = \"{Config.AppConfig.Testscript}\"");
            GenerateValid = Manager.Generate();
            GenerateTime = DateTime.Now;
        }

        DateTime _generate_time = DateTime.Now;
        public DateTime GenerateTime
        {
            get { return _generate_time; }
            private set
            {
                if (_generate_time != value)
                {
                    _generate_time = value;
                    NotifyOfPropertyChange(nameof(GenerateTime));
                }
            }
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
                    NotifyOfPropertyChange(nameof(GenerateForground));
                }
            }
        }

        public string GenerateForground
        {
            get { return (GenerateValid) ? "Black" : "Red"; }
        }

        void SetScaleModes()
        {
            X.Mode = SelectedGroupMode;
            Y.Mode = SelectedGroupMode.Covariant();
        }
    }
}
