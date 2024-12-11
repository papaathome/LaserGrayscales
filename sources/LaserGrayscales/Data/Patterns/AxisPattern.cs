using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using As.Applications.Validation;

namespace As.Applications.Data.Patterns
{
    /// <summary>
    /// Limitations on an axis for number of images, gaps and max width (or height).
    ///
    /// .ctor with attribute value injection.
    /// </remarks>
    /// <param name="imageWidth">value to use for image width (or height).</param>
    /// <param name="gap">space between two images</param>
    /// <param name="count">max number of whole images on the axis</param>
    /// <param name="max">limit on width (or heigth) on the axis</param>
    public class AxisPattern : IEquatable<AxisPattern>, IDataErrorInfo
    {
        public AxisPattern()
        {
            _dataErrorInfo = new(this);
        }

        public override string ToString() => Data.ToString.Format(nameof(AxisPattern), $"Gap={Gap}, Count={Count}");

        #region Properties
        /// <summary>
        /// space between two images, greater or equal to 0
        /// </summary>
        [XmlAttribute("gap")]
        public double Gap
        {
            get => _gap;
            set => Set(ref _gap, value, value.ValidateMinimum(0));
        }
        double _gap = 0;

        /// <summary>
        /// max number of whole images on the axis, greater or equal to 1
        /// </summary>
        [XmlAttribute("count")]
        public int Count
        {
            get => _count;
            set => Set(ref _count, value, value.ValidateMinimum(1));
        }
        int _count = 1;

        /// <summary>
        /// Width (or height) used on the axis. Width is less or equal to Max.
        /// </summary>
        public double Width(float ImageWidth) => Count * ImageWidth + Math.Max(0, Count - 1) * Gap;
        #endregion Properties

        #region IEquatable<Pattern>
        /// <inheritdoc/>
        public bool Equals(AxisPattern? other)
        {
            return
                other != null &&
                _gap.Equals(other._gap) &&
                _count.Equals(other._count);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => Equals(obj as AxisPattern);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                0x137e137e ^
                _gap.GetHashCode() ^
                _count.GetHashCode();
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
            [CallerMemberName] string propertyName = "") where T : IEquatable<T>
            => _dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName);
        #endregion
    }
}
