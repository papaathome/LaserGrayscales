using System.Xml.Serialization;

using As.Applications.Plotters;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;
using As.Applications.Validation;

namespace As.Applications.Data.Images
{
    public abstract partial class Image : IPlot
    {
        #region Properties
        [XmlIgnore]
        public GPoint Count => _count;
        GPoint _count = new GPoint() { X = 1, Y = 1 };
        #endregion Properties

        #region Actions
        /// <inheritdoc/>
        public abstract void Draw();

        /// <inheritdoc/>
        public abstract List<PlotAction> Plot();

        /// <inheritdoc/>
        public abstract List<PlotAction> PlotLine(long n);

        /// <inheritdoc/>
        public abstract List<PlotAction> PlotUnalligned();
        #endregion Actions

        #region IEquatable<IPlot>
        /// <inheritdoc/>
        public virtual bool Equals(IPlot? other)
            => Equals(other as Image);
        #endregion IEquatable<Image>
    }
}
