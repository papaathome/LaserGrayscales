using System.Globalization;
using System.Xml.Serialization;

using As.Applications.Config;
using As.Applications.Data.Scales;
using As.Applications.Validation;

namespace As.Applications.Data.Patterns
{
    public class Axis : IEquatable<Axis>, IFormattable
    {
        #region IFormattable
        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <returns>string with the readable presentation</returns>
        public override string ToString() => ToString(Settings.App.GCode.FormatFloat, CultureInfo.InvariantCulture);

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
            => $"Axis:[ {nameof(ZHeight)}:{ZHeight.ToString(format, formatProvider)}, {nameof(Speed)}:{Speed.ToString(format, formatProvider)}, {nameof(Power)}:{Power.ToString("G", formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        [XmlElement("power")]
        public Scale<int> Power
        {
            get => _power;
            set
            {
                _power = value;
                _power.ValidateMinimum = static (int v) => { return v.ValidateMinimum(0); };
                _power.ValidateMaximum = static (int v) => { return v.ValidateMinimum(0); };
            }
        }
        Scale<int> _power = new Scale<int>() { Minimum = 0, Maximum = 0 };

        [XmlElement("speed")]
        public Scale<double> Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                _speed.ValidateMinimum = static (double v) => { return v.ValidateMinimum(0); };
                _speed.ValidateMaximum = static (double v) => { return v.ValidateMinimum(0); };
            }
        }
        Scale<double> _speed = new Scale<double>() { Minimum = 0, Maximum = 0 };

        [XmlElement("z_height")]
        public Scale<double> ZHeight
        {
            get => _zHeight;
            set
            {
                _zHeight = value;
                _zHeight.ValidateMinimum = static (double v) => { return v.ValidateMinimum(0); };
                _zHeight.ValidateMaximum = static (double v) => { return v.ValidateMinimum(0); };
            }
        }
        Scale<double> _zHeight = new Scale<double>() { Minimum = 0, Maximum = 0 };
        #endregion Properties

        #region IEquatable<Axis>
        /// <inheritdoc/>
        public bool Equals(Axis? other)
        {
            return
                other != null &&
                _power.Equals(other._power) &&
                _speed.Equals(other._speed) &&
                _zHeight.Equals(other._zHeight);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => Equals(obj as Axis);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                0x15253545 ^
                _power.GetHashCode() ^
                _speed.GetHashCode() ^
                _zHeight.GetHashCode();
        }
        #endregion IEquatable<TestPattern>
    }
}
