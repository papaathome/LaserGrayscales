namespace As.Applications.Data.Images
{
    public partial class Card : Image
    {

        public Card() : base() { noEmptyImagePath = true; }

        public override string ToString() => Data.ToString.Format(nameof(Card), $"Width={Width}, Height={Height}, Lines/cm={LinesPerCm}, Image=\"{ImagePath}\"");

        #region IImage
        /// <inheritdoc/>
        public override bool Equals(Image? other)
        {
            return
                other is Card &&
                base.Equals(other) &&
                LinesPerCm.Equals((other as Square)?.LinesPerCm) &&
                ImagePath.Equals((other as Card)?.ImagePath);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                base.GetHashCode() ^
                ImageType.Card.GetHashCode() ^
                LinesPerCm.GetHashCode() ^
                ImagePath.GetHashCode();
        }
        #endregion IImage
    }
}
