using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

using As.Applications.Validation;

namespace As.Applications.Data.Patterns
{
    /// <summary>
    /// An array of n by m identical images with limits on n, m and gaps between images and where image is an IImage
    /// 
    /// .ctor with attribute value injection
    /// </remarks>
    public partial class Pattern : IEquatable<Pattern>, IDataErrorInfo
    {
        public Pattern()
        {
            _dataErrorInfo = new(this);
        }

        public override string ToString() => Data.ToString.Format(nameof(Pattern), $"X={X}, Y={Y}");

        #region Properties
        /// <summary>
        /// Limitations on the x-axis for number of images, gaps and max width.
        /// </summary>
        [XmlElement("x")]
        public AxisPattern X
        {
            get => _x;
            set => Set(ref _x, value);
        }
        AxisPattern _x = new();

        /// <summary>
        /// Limitations on the y-axis for number of images, gaps and max height.
        /// </summary>
        [XmlElement("y")]
        public AxisPattern Y
        {
            get => _y;
            set => Set(ref _y, value);
        }
        AxisPattern _y = new();
        #endregion Properties

        #region IEquatable<Pattern>
        /// <inheritdoc/>
        public bool Equals(Pattern? other)
        {
            return
                other != null &&
                _x.Equals(other._x) &&
                _y.Equals(other._y);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => Equals(obj as Pattern);

        public override int GetHashCode()
        {
            return
                _x.GetHashCode() ^
                _y.GetHashCode();
        }
        #endregion IEquatable<Pattern>

        #region IDataErrorInfo
        /// <inheritdoc/>
        readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        /// <summary>
        /// Property setter for value, error info and to fire PropertyChanged
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        internal bool Set<T>(
            ref T lvalue,
            T value,
            string? error = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T>
            => _dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName);

        /// <summary>
        /// Property setter for value, error info and to fire PropertyChanged
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        internal bool SetNullable<T>(
            ref T? lvalue,
            T? value,
            string? error = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T?>
            => _dataErrorInfo.TrySetNullable(ref lvalue, value, error, propertyName: propertyName);
        #endregion
    }
}
