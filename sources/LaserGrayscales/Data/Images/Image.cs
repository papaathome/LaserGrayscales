using System.Xml.Serialization;
using System.Runtime.CompilerServices;

using As.Applications.Validation;

namespace As.Applications.Data.Images
{
    /// <summary>
    /// Abstract class for images representing all proberties to follow the SOLID principle.
    /// Some properties make less sence for some image types but it will break the Liskov principle on inheritance if left out.
    /// </summary>
    [XmlInclude(typeof(Line))]
    [XmlInclude(typeof(Box))]
    [XmlInclude(typeof(Square))]
    [XmlInclude(typeof(Card))]
    public abstract partial class Image : IEquatable<Image>
    {
        public Image()
        {
            _dataErrorInfo = new(this);
        }

        #region Properties
        /// <inheritdoc/>
        [XmlAttribute("width")]
        public double Width
        {
            get { return _width; }
            set { Set(ref _width, value, value.ValidateMinimum(0, open: true)); }
        }
        double _width = 0.001;

        /// <inheritdoc/>
        [XmlAttribute("heigth")]
        public double Height
        {
            get { return _height; }
            set { Set(ref _height, value, value.ValidateMinimum(0, open: true)); }
        }
        double _height = 0.001;

        /// <summary>
        /// Number of lines in lines/cm
        /// </summary>
        [XmlAttribute("lines_per_cm")]
        public double LinesPerCm
        {
            get { return _lines_per_cm; }
            set { Set(ref _lines_per_cm, value, value.ValidateMinimum(0)); }
        }
        double _lines_per_cm = 0;
        public bool ShouldSerializeLinesPerCm() => (0 < LinesPerCm);

        protected bool noEmptyImagePath { get; set; } = false;

        /// <summary>
        /// Path to a file containing the image picture for filling
        /// </summary>
        [XmlAttribute("image_path")]
        public string ImagePath
        {
            get { return _imagePath; }
            set { Set(ref _imagePath, value, value.ValidateFileExists(noempty: noEmptyImagePath)); }
        }
        string _imagePath = "";
        public bool ShouldSerializeImagePath() => !string.IsNullOrWhiteSpace(ImagePath);
        #endregion Properties

        #region IEquatable<Image>
        /// <inheritdoc/>
        public virtual bool Equals(Image? other)
        {
            return
                other != null &&
                Width.Equals(other.Width) &&
                Height.Equals(other.Height);
            // note: other properties used are included by inheriting classes.
        }

        public override bool Equals(object? obj)
            => Equals(obj as Image);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                0x5a5a5a5a ^
                _width.GetHashCode() ^
                _height.GetHashCode();
            // note: other properties used are included by inheriting classes.
        }
        #endregion IEquatable<Image>

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
