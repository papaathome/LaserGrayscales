using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

using As.Applications.Config;
using As.Applications.Converters;

namespace As.Applications.Plotters.Engine.Data
{
    public class GPoint : IEquatable<GPoint>, ICloneable, INotifyPropertyChanging, IFormattable
    {
        #region Operators
        /// <summary>
        /// Addition operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public GPoint operator +(GPoint left, GPoint right)
        {
            return new GPoint()
            {
                X = left.X + right.X,
                Y = left.Y + right.Y
            };
        }

        /// <summary>
        /// Subtraction operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public GPoint operator -(GPoint left, GPoint right)
        {
            return new GPoint()
            {
                X = left.X - right.X,
                Y = left.Y - right.Y
            };
        }

        /// <summary>
        /// Skewed scalar multiplier.
        /// </summary>
        /// <param name="left">Point</param>
        /// <param name="right">scalars x and y</param>
        /// <returns>Point(X*x, Y*y)</returns>
        static public GPoint operator *(GPoint left, GPoint right)
        {
            return new GPoint()
            {
                X = left.X * right.X,
                Y = left.Y * right.Y
            };
        }

        /// <summary>
        /// scalar multiplier.
        /// </summary>
        /// <param name="left">Point</param>
        /// <param name="right">scalar c</param>
        /// <returns>Point(X*c, Y*c)</returns>
        static public GPoint operator *(GPoint left, double right)
        {
            return new GPoint()
            {
                X = left.X * right,
                Y = left.Y * right
            };
        }

        /// <summary>
        /// scalar divider.
        /// </summary>
        /// <param name="left">Point</param>
        /// <param name="right">scalar c</param>
        /// <returns>Point(X/c, Y/c)</returns>
        static public GPoint operator /(GPoint left, double right)
        {
            return new GPoint()
            {
                X = left.X / right,
                Y = left.Y / right
            };
        }
        #endregion Operators

        public object Clone() => new GPoint() { X = X, Y = Y };

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
             => $"[ X:{X.Ci(format, formatProvider)}, Y:{Y.Ci(format, formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        [XmlAttribute("x")]
        public double X
        {
            get => _x;
            set
            {
                if (_x.Equals(value)) return;
                _x = value;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(X)));
            }
        }
        double _x = 0;

        [XmlAttribute("y")]
        public double Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Y)));
            }
        }
        double _y = 0;

        public bool IsZero => ((X == 0) && (Y == 0));
        #endregion Properties

        #region INotifyPropertyChanging
        // Please note that this is not INotifyPropertyChanged

        public event PropertyChangingEventHandler? PropertyChanging;
        #endregion INotifyPropertyChanging

        #region IEquatable<Point>
        public bool Equals(GPoint? other)
        {
            if (other == null) return false;
            return
                X.Equals(other.X) &&
                Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not GPoint) return false;
            return Equals(obj as GPoint);
        }

        public override int GetHashCode()
        {
            int result = 0x75a5a5a5;
            result ^= X.GetHashCode();
            result ^= Y.GetHashCode();
            return result;
        }
        #endregion IEquatable<Point>
    }
}
