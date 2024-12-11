using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using As.Applications.Converters;
using As.Applications.Validation;

namespace As.Applications.Data.Scales
{
    public partial class Scale<T> : IEquatable<Scale<T>>, IDataErrorInfo, IFormattable
        where T : notnull, INumber<T>, IFormattable
    {
        public delegate string OnValidateValue(T value);

        public Scale()
        {
            _dataErrorInfo = new(this);
        }

        #region IFormattable
        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <returns>string with the readable presentation</returns>
        public override string ToString() => ToString("G", CultureInfo.InvariantCulture);

        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <param name="format">format of min and max values, defaults to "G"</param>
        /// <returns>string with the readable presentation</returns>
        public string ToString(string? format) => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <param name="format">format of min and max values, defaults to "G"</param>
        /// <param name="formatProvider">format provider default to CultureInfo.InvariantCulture</param>
        /// <returns>string with the readable presentation</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => $"{nameof(Scale<T>)}:[ {nameof(Minimum)}={Minimum.Ci(format, formatProvider)}, {nameof(Maximum)}={Maximum.Ci(format, formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        [XmlAttribute("minimum")]
        public required T Minimum
        {
            get => _minimum;
            set => Set(ref _minimum, value, ValidateMinimum.Invoke(value));
        }
        T _minimum = default!;

        /// <summary>
        /// Validate (T)value for Minimum
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <returns>Empty string if accepted, error message otherwise</returns>
        [XmlIgnore]
        public OnValidateValue ValidateMinimum
        {
            get => _on_validate_minimum;
            set
            {
                if (_on_validate_minimum == value) return;
                _on_validate_minimum = value;
                this[nameof(Minimum)] = _on_validate_minimum.Invoke(Minimum);
            }
        }
        OnValidateValue _on_validate_minimum = (T value) => "";

        [XmlAttribute("maximum")]
        public required T Maximum
        {
            get => _maximum;
            set => Set(ref _maximum, value, ValidateMaximum(value));
        }
        T _maximum = default!;

        /// <summary>
        /// Validate (T)value for Maximum
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <returns>Empty string if accepted, error message otherwise</returns>
        [XmlIgnore]
        public OnValidateValue ValidateMaximum
        {
            get => _on_validate_maximum;
            set
            {
                if (_on_validate_maximum == value) return;
                _on_validate_maximum = value;
                this[nameof(Maximum)] = _on_validate_maximum.Invoke(Maximum);
            }
        }
        OnValidateValue _on_validate_maximum = (T value) => "";
        #endregion Properties

        #region IEquatable<Scale<T>>
        /// <inheritdoc/>
        public bool Equals(Scale<T>? other)
        {
            return
                other != null &&
                Minimum.Equals(other.Minimum) &&
                Maximum.Equals(other.Maximum);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => Equals(obj as Scale<T>);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                0x25a5a5a5 ^
                Minimum.GetHashCode() ^
                Maximum.GetHashCode();
        }
        #endregion IEquatable<TestPattern>

        #region IDataErrorInfo
        /// <inheritdoc/>
        protected readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        /// <summary>
        /// Property setter for value and error info
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        internal virtual bool Set(
            ref T lvalue,
            T value,
            string? error = null,
            [CallerMemberName] string propertyName = "")
            => _dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName);
        #endregion
    }
}
