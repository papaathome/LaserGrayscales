namespace As.Applications.Data.Images
{
    public partial class Square() : Image
    {
        public override string ToString() => Data.ToString.Format(nameof(Square), $"Width={Width}, Height={Height}, Lines/cm={LinesPerCm}");

        #region IImage
        /// <inheritdoc/>
        public override bool Equals(Image? other)
        {
            return
                other is Square &&
                base.Equals(other) &&
                LinesPerCm.Equals((other as Square)?.LinesPerCm);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                ImageType.Square.GetHashCode() ^
                LinesPerCm.GetHashCode();
        }
        #endregion IImage
    }
}
