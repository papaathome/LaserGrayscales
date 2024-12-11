using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

using As.Applications.Config;
using As.Applications.Data.Images;
using As.Applications.Data.Scales;
using As.Applications.Validation;

namespace As.Applications.Data.Patterns
{
    /// <summary>
    /// An array of n by m identical images with limits on n, m and grouped gaps between images and where image is an IImage
    /// 
    /// .ctor with attribute value injection.
    /// </summary>
    /// <param name="innerPattern"></param>
    /// <param name="outerPattern"></param>
    [XmlRoot("testpattern")]
    public sealed partial class TestPattern : SettingsBase, IEquatable<TestPattern>
    {
        // TODO: generate image and create image view.
        // TODO: generate plot and gcode view
        // TODO: add home location setting.
        // TODO: add offset setting.
        // TODO: add switch for imperial settings.

        public TestPattern()
        {
            initialing = false;
        }

        protected override void Initialise()
        {
            Image = new Line();

            InnerPattern = new Pattern();
            OuterPattern = new Pattern();

            SetValue(
                nameof(SpeedG0Set),
                "70",
                "F Speed setting for G0 traveling in mm/sec.",
                ValidString.ValidateIsIntValue);
        }

        #region Properties
        readonly bool initialing = true;

        /// <summary>
        /// Speed for G0 use.
        /// </summary>
        [XmlIgnore]
        public double SpeedG0Set => Get<double>();

        [XmlElement("image")]
        public Image Image
        {
            get { return _image; }
            set
            {
                if (!Set(ref _image, value, forceUpdate: initialing)) return;
                InnerPattern.Image = Image;
            }
        }
        Image _image = new Line();

        [XmlElement("inner_pattern")]
        public Pattern InnerPattern
        {
            get { return _innerPattern; }
            set
            {
                if (!Set(ref _innerPattern, value, forceUpdate: initialing)) return;
                InnerPattern.Image = Image;
                OuterPattern.Image = InnerPattern;
            }
        }
        Pattern _innerPattern = new();

        [XmlElement("outer_pattern")]
        public Pattern OuterPattern
        {
            get => _outerPattern;
            set
            {
                if (!Set(ref _outerPattern, value, forceUpdate: initialing)) return;
                OuterPattern.Image = InnerPattern;
            }
        }
        Pattern _outerPattern = new();

        /// <summary>
        /// X axis minimum and maximum values for ZHeight, Speed and Power
        /// </summary>
        [XmlElement("x_axis")]
        public Axis XLimits
        {
            get => _xLimits;
            set => Set(ref _xLimits, value);
        }
        Axis _xLimits = new Axis()
        {
            Power = new Scale<int>() { Minimum = 0, Maximum = 0 },
            Speed = new Scale<double>() { Minimum = 1, Maximum = 1 },
            ZHeight = new Scale<double>() { Minimum = 0, Maximum = 0 }
        };

        /// <summary>
        /// Y axis minimum and maximum values for ZHeight, Speed and Power
        /// </summary>
        [XmlElement("y_axis")]
        public Axis YLimits
        {
            get => _yLimits;
            set => Set(ref _yLimits, value);
        }
        Axis _yLimits = new Axis()
        {
            Power = new Scale<int>() { Minimum = 0, Maximum = 0 },
            Speed = new Scale<double>() { Minimum = 1, Maximum = 1 },
            ZHeight = new Scale<double>() { Minimum = 0, Maximum = 0 }
        };
        #endregion Properties

        #region IEquatable<TestPattern>
        /// <inheritdoc/>
        public bool Equals(TestPattern? other)
        {
            return
                other != null &&
                _innerPattern.Equals(other._innerPattern) &&
                _outerPattern.Equals(other._outerPattern);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
            => Equals(obj as TestPattern);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                0x25a5a5a5 ^
                _innerPattern.GetHashCode() ^
                _outerPattern.GetHashCode();
        }
        #endregion IEquatable<TestPattern>

        #region IDataErrorInfo
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
            bool forceUpdate = false,
            [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (!_dataErrorInfo.TrySet(ref lvalue, value, error, forceUpdate, null, propertyName)) return false;
            return true;
        }
        #endregion
    }
}
