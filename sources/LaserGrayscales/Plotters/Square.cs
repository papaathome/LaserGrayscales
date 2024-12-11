using As.Applications.Plotters;
using As.Applications.Plotters.Engine;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Data.Images
{
    public partial class Square : IPlot
    {
        /// <inheritdoc/>
        public override void Draw() { }

        /// <inheritdoc/>
        public override List<PlotAction> Plot()
        {
            List<PlotAction> result =
            [
                new Plot() { Home = Reference.Relative, P = new GPoint() { X = Width, Y = 0 } },
                new Plot() { Home = Reference.Relative, P = new GPoint() { X = 0, Y = Height } },
                new Plot() { Home = Reference.Relative, P = new GPoint() { X = -Width, Y = 0 } },
                new Plot() { Home = Reference.Relative, P = new GPoint() { X = 0, Y = -Height } }
            ];
            var h = 10.0 / LinesPerCm;
            for (var y = h; y < Height; y+=h)
            {
                result.Add(new Move() { Home = Reference.Relative, P = new GPoint() { X = 0, Y = h } });
                result.Add(new Plot() { Home = Reference.Relative, P = new GPoint() { X = Width, Y = 0 } });
                result.Add(new Move() { Home = Reference.Relative, P = new GPoint() { X = -Width, Y = 0 } });
            }
            return result;
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
