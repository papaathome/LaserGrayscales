using System.Xml.Serialization;

namespace As.Applications.Data.Images
{
    [XmlRoot("box")]
    public partial class Box() : Image
    {
        public override string ToString() => Data.ToString.Format(nameof(Box), $"Width={Width}, Height={Height}");

        #region IImage
        /// <inheritdoc/>
        public override bool Equals(Image? other)
        {
            return
                other is Box &&
                base.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                ImageType.Box.GetHashCode();
        }
        #endregion IImage
    }
}
