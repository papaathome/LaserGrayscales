using System.Numerics;

namespace As.Applications.Data.Scales
{
    public partial class Scale<T> where T : notnull, INumber<T>, IFormattable
    {
        #region Actions
        public T Value(T index, T max_index)
            => Minimum + (Maximum - Minimum) * index / max_index;

        public T Clip(T value)
        {
            if (value < Minimum) return Minimum;
            if (Maximum < value) return Maximum;
            return value;
        }
        #endregion Actions
    }
}
