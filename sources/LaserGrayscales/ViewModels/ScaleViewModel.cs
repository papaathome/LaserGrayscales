using System.ComponentModel;

using Caliburn.Micro;
using As.Applications.Models;
using As.Applications.Procedures;
using As.Applications.Data;

namespace As.Applications.ViewModels
{
    internal class ScaleViewModel : Screen, IDataErrorInfo, IViewValidInfo
    {
        public ScaleViewModel(Scale scale, Mode mode)
        {
            Scale = scale;
            Mode = mode;

            ValidateFirst();
            ValidateLast();
            ValidateStep();
            ValidateIncrement();
        }

        readonly Scale Scale;

        #region IDataErrorInfo
        public string Error => string.Empty;

        public string this[string item]
        {
            get
            {
                return item switch
                {
                    nameof(First) => ValidateFirst(),
                    nameof(Last) => ValidateLast(),
                    nameof(Step) => ValidateStep(),
                    nameof(Increment) => ValidateIncrement(),
                    _ => string.Empty,
                };
            }
        }
        #endregion IDataErrorInfo

        #region IViewValidInfo
        bool _isvalid_view = false;
        public bool IsValidView
        {
            get { return _isvalid_view; }
            private set
            {
                if (_isvalid_view != value)
                {
                    _isvalid_view = value;
                    OnIsValidViewChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event IsValidViewChanged? OnIsValidViewChanged;

        void ValidateView()
        {
            IsValidView =
                IsValidFirst &&
                IsValidLast &&
                IsValidStep &&
                IsValidIncrement;
        }
        #endregion IViewValidInfo

        #region Data
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

        bool _isvalid_first = false;
        bool IsValidFirst
        {
            get { return _isvalid_first; }
            set
            {
                if (_isvalid_first != value)
                {
                    _isvalid_first = value;
                    ValidateView();
                }
            }
        }

        string ValidateFirst()
        {
            var result = string.Empty;
            IsValidFirst = First.TryIsValidRange(Minimum, Maximum, ref result);
            return result;
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

        bool _isvalid_last = false;
        bool IsValidLast
        {
            get { return _isvalid_last; }
            set
            {
                if (_isvalid_last != value)
                {
                    _isvalid_last = value;
                    ValidateView();
                }
            }
        }

        string ValidateLast()
        {
            var result = string.Empty;
            IsValidLast = Last.TryIsValidRange(Minimum, Maximum, ref result);
            return result;
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

        bool _isvalid_step = false;
        bool IsValidStep
        {
            get { return _isvalid_step; }
            set
            {
                if (_isvalid_step != value)
                {
                    _isvalid_step = value;
                    ValidateView();
                }
            }
        }

        string ValidateStep()
        {
            var result = string.Empty;
            IsValidStep = Step.TryIsValidMinimum(0, ref result, open_interval:true);
            return result;
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

        bool _isvalid_increment = false;
        bool IsValidIncrement
        {
            get { return _isvalid_increment; }
            set
            {
                if (_isvalid_increment != value)
                {
                    _isvalid_increment = value;
                    ValidateView();
                }
            }
        }

        string ValidateIncrement()
        {
            var result = string.Empty;
            IsValidIncrement = Increment.TryIsValidMinimum(0, ref result, open_interval: true);
            return result;
        }
        #endregion Data
    }
}
