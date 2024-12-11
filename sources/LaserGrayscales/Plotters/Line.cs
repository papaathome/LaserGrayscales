using As.Applications.Plotters;
using As.Applications.Plotters.Engine;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Data.Images
{
    public partial class Line : IPlot
    {
        /// <inheritdoc/>
        public override void Draw() { }

        /// <inheritdoc/>
        public override List<PlotAction> Plot()
        {
            return
            [
                new Plot() { Home = Reference.Relative, P = new GPoint() { X = Width, Y = 0 } },
            ];
        }

        /// <inheritdoc/>
        public override List<PlotAction> PlotLine(long n)
        {
            return [];
        }

        /// <inheritdoc/>
        public override List<PlotAction> PlotUnalligned()
        {
            return [];
        }
    }
}
