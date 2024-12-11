using System.ComponentModel;
using System.Globalization;

using As.Applications.Config;
using As.Applications.Converters;

namespace As.Applications.Plotters.Engine.Data
{
    internal class Register : GValue, IEquatable<Register>, IFormattable
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
        public override string ToString(string? format) => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Readable presentation of format data.
        /// </summary>
        /// <param name="format">format of min and max values, defaults to "G"</param>
        /// <param name="formatProvider">format provider default to CultureInfo.InvariantCulture</param>
        /// <returns>string with the readable presentation</returns>
        public override string ToString(string? format, IFormatProvider? formatProvider)
            => $"{nameof(Register)}:[ {nameof(Value)}={Value.ToString(format, formatProvider)}, {nameof(Count)}={Count.Ci("0", formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        Stack<double> Stack { get; set; } = new Stack<double>();

        public int Count => Stack.Count;
        #endregion Properties

        #region Actions
        public void Push() => Stack.Push(Value);

        public void Pop() => Value = Stack.Pop();

        public double Peek() => Stack.Peek();
        #endregion Actions

        #region IEquatable<Register>
        public bool Equals(Register? other)
        {
            if (other == null) return false;
            return Value.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not Register)) return false;
            return Equals(obj as Register);
        }

        public override int GetHashCode() => Value.GetHashCode();
        #endregion IEquatable<Register>
    }

    internal class Register<T> : IEquatable<T>, IFormattable where T : class, IEquatable<T>, ICloneable, INotifyPropertyChanging, IFormattable
    {
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
            => $"{nameof(Register<T>)}:[ {nameof(Value)}={Value.ToString(format, formatProvider)}, {nameof(Count)}={Count.Ci("0", formatProvider)} ]";
        #endregion IFormattable

        #region Properties
        public required T Value
        {
            get => _value!;
            set
            {
                if ((_value == null) && (value == null)) return;
                if (_value != null)
                {
                    if (_value.Equals(value)) return;
                    _value.PropertyChanging -= PropertyChangingEventHandler;
                }
                _value = value;
                if (_value != null) _value.PropertyChanging += PropertyChangingEventHandler;
                Changed = true;
            }
        }
        T? _value = null;

        public bool Changed { get; private set; } = true;

        Stack<T> Stack { get; set; } = new Stack<T>();

        public int Count => Stack.Count;
        #endregion Properties

        #region Actions
        void PropertyChangingEventHandler(object? sender, PropertyChangingEventArgs e)
            => Changed = true;

        /// <summary>
        /// Reset Changed status to false.
        /// </summary>
        public void Reset() => Changed = false;

        public void Push() => Stack.Push((T)Value.Clone());

        public void Pop() => Value = Stack.Pop();

        public T Peek() => Stack.Peek();
        #endregion Actions

        #region IEquatable<T>
        public bool Equals(T? other)
        {
            if (other == null) return false;
            return Value.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not T)) return false;
            return Equals(obj as T);
        }

        public override int GetHashCode() => Value.GetHashCode();
        #endregion IEquatable<T>
    }
}
