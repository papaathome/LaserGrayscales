using System.ComponentModel;
using System.Runtime.CompilerServices;

using As.Applications.Data.Patterns;
using As.Applications.Validation;

using Caliburn.Micro;

namespace As.Applications.ViewModels
{
    public class PatternViewModel : Screen, IDataErrorInfo
    {
        public PatternViewModel()
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

        /// <summary>
        /// X-gap in mm, space between two images, greater or equal to 0
        /// </summary>
        public double XGap
        {
            get => _xgap;
            set => Set(ref _xgap, value, value.ValidateMinimum(0));
        }
        double _xgap = 0;

        /// <summary>
        /// X-count in mm/mm, max number of whole images, greater or equal to 1
        /// </summary>
        public int XCount
        {
            get { return _xcount; }
            set { Set(ref _xcount, value, value.ValidateMinimum(1)); }
        }
        int _xcount = 1;

        /// <summary>
        /// Y-gap in mm, space between two images, greater or equal to 0
        /// </summary>
        public double YGap
        {
            get => _ygap;
            set => Set(ref _ygap, value, value.ValidateMinimum(0));
        }
        double _ygap = 0;

        /// <summary>
        /// Y-count in mm/mm, max number of whole images, greater or equal to 1
        /// </summary>
        public int YCount
        {
            get { return _ycount; }
            set { Set(ref _ycount, value, value.ValidateMinimum(1)); }
        }
        int _ycount = 1;
        #endregion Properties

        #region Actions
        public Pattern GetPattern()
        {
            return new Pattern()
            {
                X = new AxisPattern()
                {
                    Gap = XGap,
                    Count = XCount,
                },
                Y = new AxisPattern()
                {
                    Gap = YGap,
                    Count = YCount,
                }
            };
        }

        public void SetPattern(Pattern pattern)
        {
            XGap = pattern.X.Gap;
            XCount = pattern.X.Count;
            YGap = pattern.Y.Gap;
            YCount = pattern.Y.Count;
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
