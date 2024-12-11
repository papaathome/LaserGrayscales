using System.Xml.Serialization;

namespace As.Applications.Data.Images
{
    [XmlRoot("line")]
    public partial class Line() : Image
    {
        public override string ToString() => Data.ToString.Format(nameof(Line), $"Width={Width}, Height={Height}" );

        #region IImage
        /// <inheritdoc/>
        public override bool Equals(Image? other)
        {
            return
                other is Line &&
                base.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                ImageType.Line.GetHashCode();
        }
        #endregion IImage
    }
}
