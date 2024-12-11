using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

using As.Applications.Config;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine
{
    public class Grid : IEquatable<Grid>, ICloneable, INotifyPropertyChanging, IFormattable
    {
        #region Operators
        static public Grid operator +(Grid left, GPoint right)
        {
            return new Grid()
            {
                P0 = left.P0 + right,
                e = (GPoint)left.e.Clone()
            };
        }

        static public Grid operator -(Grid left, GPoint right)
        {
            return new Grid()
            {
                P0 = left.P0 - right,
                e = (GPoint)left.e.Clone()
            };
        }

        /// <summary>
        /// Convert to Grid operator
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        static public GPoint operator *(Grid left, GPoint right) => left.ConvertToGrid(right);
        #endregion Operators

        public Grid()
        {
            _p0 = new GPoint() { X = 0, Y = 0 };
            _p0.PropertyChanging += PropertyChangingEventHandler;

            _e = new GPoint() { X = 0, Y = 0 };
            _e.PropertyChanging += PropertyChangingEventHandler;
        }

        public object Clone() => new Grid() { P0 = (GPoint)P0.Clone(), e = (GPoint)e.Clone() };

        #region IForattable
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
            => $"{nameof(Grid)}:[ {nameof(P0)}={P0.ToString(format, formatProvider)}, {nameof(e)}={e.ToString(format, formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        /// <summary>
        /// Grid (0, 0) in machine coordinates.
        /// </summary>
        ///
        [XmlElement("p0")]
        public GPoint P0
        {
            get => _p0;
            set
            {
                if (_p0 == value) return;
                if (_p0 != null) _p0.PropertyChanging -= PropertyChangingEventHandler;
                _p0 = value;
                if (_p0 != null) _p0.PropertyChanging += PropertyChangingEventHandler;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(P0)));
            }
        }
        GPoint _p0;

        /// <summary>
        /// Grid |e_u|/|e_x| and |e_v|/|e_y| length conversions
        /// </summary>
        [XmlElement("e")]
        public GPoint e
        {
            get => _e;
            set
            {
                if (_e == value) return;
                if (_e != null) _e.PropertyChanging -= PropertyChangingEventHandler;
                _e = value;
                if (_e != null) _e.PropertyChanging += PropertyChangingEventHandler;
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(e)));
            }
        }
        GPoint _e;
        #endregion Properties

        #region Actions
        public event PropertyChangingEventHandler? PropertyChanging;

        void PropertyChangingEventHandler(object? sender, PropertyChangingEventArgs e)
        {
            if (sender is not GPoint) return;
            var p = sender as GPoint;
            if (sender == _p0) PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(P0)));
            if (sender == _e) PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(e)));
        }

        public GPoint ConvertToGrid(GPoint machine_point)
        {
            return new GPoint()
            {
                X = e.X*(machine_point.X - P0.X),
                Y = e.Y*(machine_point.Y - P0.Y),
            };
        }

        public GPoint Round_ConvertToGrid(GPoint machine_point) => Round(ConvertToGrid(machine_point));

        public GPoint Truncate_ConvertToGrid(GPoint machine_point) => Truncate(ConvertToGrid(machine_point));

        public GPoint ConvertFromGrid(GPoint grid_point)
        {
            return new GPoint()
            {
                X = grid_point.X / e.X + P0.X,
                Y = grid_point.Y / e.Y + P0.Y,
            };
        }

        /// <summary>
        /// Rounds grid_point to the nearest integral value, and rounds midpoint values to the nearest even number.
        /// </summary>
        /// <param name="grid_point"></param>
        /// <returns></returns>
        static public GPoint Round(GPoint grid_point)
        {
            return new GPoint()
            {
                X = Math.Round(grid_point.X),
                Y = Math.Round(grid_point.Y)
            };
        }

        /// <summary>
        /// rounds grid_point to the nearest integer towards zero.
        /// </summary>
        /// <param name="grid_point"></param>
        /// <returns></returns>
        static public GPoint Truncate(GPoint grid_point)
        {
            return new GPoint()
            {
                X = Math.Truncate(grid_point.X),
                Y = Math.Truncate(grid_point.Y)
            };
        }
        #endregion Actions

        #region IEquatable<Grid>
        public bool Equals(Grid? other)
        {
            if (other == null) return false;
            return
                P0.Equals(other.P0) &&
                e.Equals(other.e);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not Grid)) return false;
            return Equals(obj as Grid);
        }

        public override int GetHashCode()
        {
            int result = 0x3a3a3a3a;
            result ^= P0.GetHashCode();
            result ^= e.GetHashCode();
            return result;
        }
        #endregion IEquatable<Point>
    }
}
