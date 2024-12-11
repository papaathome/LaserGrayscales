using System.ComponentModel;

using As.Applications.Appenders;
using As.Applications.Loggers;

namespace As.Applications.ViewModels
{
    internal class ConsoleViewModel : INotifyPropertyChanged
    {
        public ConsoleViewModel()
        {
            UI.OnUiEventHandler += OnUiEvent;
        }

        #region Properties
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
        string _console = string.Empty;
        #endregion Properties

        #region Actions
        void OnUiEvent(object? sender, MessageLoggedEventArgs e)
            => Writeln(e.RenderedLoggingEvent);

        void Writeln(string message)
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

        public void Clear() => Console = string.Empty;
        #endregion Actions

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged
    }
}
