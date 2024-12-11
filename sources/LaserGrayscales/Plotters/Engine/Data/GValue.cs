using System.Globalization;

using As.Applications.Config;
using As.Applications.Converters;

namespace As.Applications.Plotters.Engine.Data
{
    public class GValue : IEquatable<GValue>, IFormattable
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
        public virtual string ToString(string? format) => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <param name="format">format of min and max values, defaults to "G"</param>
        /// <param name="formatProvider">format provider default to CultureInfo.InvariantCulture</param>
        /// <returns>string with the readable presentation</returns>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
        {
            var v = (Fractional) ? Value : Math.Truncate(Value);
            var f = (Fractional) ? format : "0";
            return $"{nameof(GValue)}:[ {nameof(GName)}={GName}, {nameof(Value)}={v.ToString(f, formatProvider)}, {nameof(Factor)}={Factor.ToString(format, formatProvider)} ]";
        }
        #endregion IFormattable

        #region Properties
        public required string GName { get; set; }

        public double Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value)) return;
                _value = value;
                Changed = true;
            }
        }
        double _value = 0;

        public double Factor { get; set; } = 1.0;

        public bool Fractional { get; set; } = true;

        public bool Changed { get; private set; } = true;
        #endregion Properties

        #region Actions
        public void Reset() => Changed = false;

        internal void WriteGcode(Context context, bool forced = false)
        {
            if (Changed | forced)
            {
                context.Write(" ");
                context.Write(FormatGCode(Value));
                Changed = false;
            }
        }

        internal void WriteGcode(Context context, double value)
        {
            context.Write(" ");
            context.Write(FormatGCode(value));
            Changed = true;
        }

        string FormatGCode(double value)
        {
            var v = (Fractional) ? (Factor * value) : Math.Truncate(Factor * value);
            var f = (Fractional) ? Settings.App.GCode.FormatFloat : "0";
            return $"{GName}{(Factor * value).Ci(f)}";
        }
        #endregion Actions

        #region IEquatable<Register>
        public bool Equals(GValue? other)
        {
            if (other == null) return false;
            return Value.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not GValue)) return false;
            return Equals(obj as GValue);
        }

        public override int GetHashCode() => Value.GetHashCode();
        #endregion IEquatable<Register>
    }
}
