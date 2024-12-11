using System.ComponentModel;
using System.Runtime.CompilerServices;

using As.Applications.Data.Scales;
using As.Applications.Validation;

using Caliburn.Micro;

namespace As.Applications.ViewModels
{
    public class ScaleViewModel : Screen, IDataErrorInfo
    {
        public ScaleViewModel()
        {
            _dataErrorInfo = new(this);
        }

        #region Properties
        public string Name 
        {
            get => _name;
            set => Set(ref _name, value);
        }
        string _name = "";

        public int PowerMinimum
        {
            get => _power_minimum;
            set => Set(ref _power_minimum, value, value.ValidateRange(0, 255));
        }
        int _power_minimum = 0;

        public int PowerMaximum
        {
            get => _power_maximum;
            set => Set(ref _power_maximum, value, value.ValidateRange(0, 255));
        }
        int _power_maximum = 0;

        public double SpeedMinimum
        {
            get => _speed_minimum;
            set => Set(ref _speed_minimum, value, value.ValidateMinimum(0));
        }
        double _speed_minimum = 0.001;

        public double SpeedMaximum
        {
            get => _speed_maximum;
            set => Set(ref _speed_maximum, value, value.ValidateMinimum(0));
        }
        double _speed_maximum = 0.001;

        public double HeightMinimum
        {
            get => _height_minimum;
            set => Set(ref _height_minimum, value, value.ValidateMinimum(0));
        }
        double _height_minimum = 0;

        public double HeightMaximum
        {
            get => _height_maximum;
            set => Set(ref _height_maximum, value, value.ValidateMinimum(0));
        }
        double _height_maximum = 0;
        #endregion Properties

        #region Actions
        public Scale<int> GetPowerScale()
        {
            return new Scale<int>()
            {
                Minimum = PowerMinimum,
                Maximum = PowerMaximum
            };
        }

        public void SetPowerScale(Scale<int> value)
        {
            PowerMinimum = value.Minimum;
            PowerMaximum = value.Maximum;
        }

        public Scale<double> GetSpeedScale()
        {
            return new Scale<double>()
            {
                Minimum = SpeedMinimum,
                Maximum = SpeedMaximum
            };
        }

        public void SetSpeedScale(Scale<double> value)
        {
            SpeedMinimum = value.Minimum;
            SpeedMaximum = value.Maximum;
        }

        public Scale<double> GetHeightScale()
        {
            return new Scale<double>()
            {
                Minimum = HeightMinimum,
                Maximum = HeightMaximum
            };
        }

        public void SetHeightScale(Scale<double> value)
        {
            HeightMinimum = value.Minimum;
            HeightMaximum = value.Maximum;
        }
        #endregion Actions

        #region IDataErrorInfo
        readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        internal bool Set<T>(ref T lvalue, T value, string? error = null, [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (!_dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion
    }
}
