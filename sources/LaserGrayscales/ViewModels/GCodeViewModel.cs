using As.Applications.Config;

namespace As.Applications.ViewModels
{
    class GCodeViewModel : KvViewModel
    {
        public GCodeViewModel()
        {
            // Can't use this.KV, not yet initialised.
            GCodeKv = new KvViewModel() { KV = Settings.App.GCode.Content };
        }

        #region Properties
        public required GCodeSettings GCode
        {
            get => _gCode!;
            set
            {
                if (_gCode != null) _gCode.PropertyChanged -= PropertyChangedHandler;
                _gCode = value;
                if (_gCode != null) _gCode.PropertyChanged += PropertyChangedHandler;
            }
        }
        GCodeSettings? _gCode = null;

        public string Intro
        {
            get => GCode.Intro;
            set => GCode.Intro = value;
        }

        public string Header
        {
            get => GCode.Header;
            set => GCode.Header = value;
        }

        public string Footer
        {
            get => GCode.Footer;
            set => GCode.Footer = value;
        }

        public KvViewModel GCodeKv { get; private set; }
        #endregion Properties

        #region Actions
        private void PropertyChangedHandler(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(GCodeSettings.Intro): NotifyOfPropertyChange(nameof(Intro)); break;
                case nameof(GCodeSettings.Header): NotifyOfPropertyChange(nameof(Header)); break;
                case nameof(GCodeSettings.Footer): NotifyOfPropertyChange(nameof(Footer)); break;
            }
        }
        #endregion Actions
    }
}
