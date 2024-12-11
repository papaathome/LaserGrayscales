using As.Applications.Plotters;
using As.Applications.Plotters.Engine;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Data.Images
{
    public partial class Card : IPlot
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
            for (var y = h; h < Height; y += h)
            {
                result.Add(new Move() { P = new GPoint() { X = 0, Y = y } });
                for (var x = 0.0; x < Width; x += h)
                {
                    // TODO: modulate plot laser power based on image pixel gray level.
                    result.Add(new Plot() { Home = Reference.Relative, P = new GPoint() { X = x, Y = y } });
                }
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
