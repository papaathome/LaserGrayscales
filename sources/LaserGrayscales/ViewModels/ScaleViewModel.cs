using Caliburn.Micro;
using As.Applications.Models;
using System.ComponentModel;

namespace As.Applications.ViewModels
{
    internal class ScaleViewModel : Screen, IDataErrorInfo
    {
        public ScaleViewModel(Scale scale, Mode mode)
        {
            Scale = scale;
            Mode = mode;
        }

        public string Error => string.Empty;

        public string this[string item]
        {
            get
            {
                return item switch
                {
                    nameof(First) => ValidateRange(First, Minimum, Maximum),
                    nameof(Last) => ValidateRange(Last, Minimum, Maximum),
                    _ => string.Empty,
                };
            }
        }

        readonly Scale Scale;

        Mode _mode;
        public Mode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    NotifyOfPropertyChange(nameof(Mode));
                    Minimum = value.Minimum();
                    Maximum = value.Maximum();
                }
            }
        }

        int _minimum = 0;
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;
                    NotifyOfPropertyChange(nameof(Minimum));
                    NotifyOfPropertyChange(nameof(First));
                    NotifyOfPropertyChange(nameof(Last));
                }
            }
        }

        int _maxmimum = 100;
        public int Maximum
        {
            get { return _maxmimum; }
            set
            {
                if (_minimum != value)
                {
                    _maxmimum = value;
                    NotifyOfPropertyChange(nameof(Maximum));
                    NotifyOfPropertyChange(nameof(First));
                    NotifyOfPropertyChange(nameof(Last));
                }
            }
        }

        public int First
        {
            get => Scale.First;
            set
            {
                if (Scale.First != value)
                {
                    Scale.First = value;
                    NotifyOfPropertyChange(nameof(First));
                }
            }
        }

        public int Last
        {
            get => Scale.Last;
            set
            {
                if (Scale.Last != value)
                {
                    Scale.Last = value;
                    NotifyOfPropertyChange(nameof(Last));
                }
            }
        }

        public int Step
        {
            get => Scale.Step;
            set
            {
                if (Scale.Step != value)
                {
                    Scale.Step = value;
                    NotifyOfPropertyChange(nameof(Step));
                }
            }
        }

        public double Increment
        {
            get => Scale.Increment;
            set
            {
                if (Scale.Increment != value)
                {
                    Scale.Increment = value;
                    NotifyOfPropertyChange(nameof(Increment));
                }
            }
        }

        static string ValidateRange(int value, int minimum, int maximum)
        {
            return
                  (value < minimum)  ? $"Value {value} is less than minimum of {minimum}"
                : (maximum < value ) ? $"Maximum of {maximum} is less than value {value}"
                : "";
        }
    }
}
