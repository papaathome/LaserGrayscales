using System.Xml.Serialization;

using As.Applications.Converters;
using As.Applications.Plotters;
using As.Applications.Plotters.Engine;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Data.Patterns
{
    public sealed partial class TestPattern : IPlot
    {
        #region Properties
        /// <inheritdoc/>
        [XmlIgnore]
        public double Width => OuterPattern?.Width ?? 0;

        /// <inheritdoc/>
        [XmlIgnore]
        public double Height => OuterPattern?.Height ?? 0;

        [XmlIgnore]
        public GPoint Count
        {
            get
            {
                _count.X = OuterPattern.Count.X;
                _count.Y = OuterPattern.Count.Y;
                return _count;
            }
        }
        GPoint _count = new GPoint() { X = 0, Y = 0 };
        #endregion Properties

        #region Actions
        /// <inheritdoc/>
        public void Draw() { }

        /// <inheritdoc/>
        public List<PlotAction> Plot()
        {
            List<PlotAction> result =
            [
                new Comment() { Message = $"{nameof(Image)}={Image}"},
                new Comment() { Message = $"{nameof(InnerPattern)}={InnerPattern}"},
                new Comment() { Message = $"{nameof(OuterPattern)}={OuterPattern}"},
                new Comment() { Message = $"X scale Height: {XLimits.ZHeight.ToString("0.000")} in mm" },
                new Comment() { Message = $"X scale Speed: {XLimits.Speed.ToString("0.000")} in mm/sec" },
                new Comment() { Message = $"X scale Power: {XLimits.Power.ToString("0.000")} in parts of 100%/255" },
                new Comment() { Message = $"Y scale Height: {YLimits.ZHeight.ToString("0.000")} in mm" },
                new Comment() { Message = $"Y scale Speed: {YLimits.Speed.ToString("0.000")} in mm/sec" },
                new Comment() { Message = $"Y scale Power: {YLimits.Power.ToString("0.000")}  in parts of 100%/255" },
            ];

            result.Add(new RegOp<Grid>()
            {
                Op = Op.Set,
                Reg = Reg.Grid,
                Value = new Grid()
                {
                    P0 = new GPoint() { X = 0, Y = 0 },
                    e = new GPoint()
                    {
                        X = 10 / ((0 < Image.LinesPerCm) ? Image.LinesPerCm : 1),
                        Y = 10 / ((0 < Image.LinesPerCm) ? Image.LinesPerCm : 1)
                    }
                }
            });
            result.Add(new RegOp<Grid>()
            {
                Op = Op.Push,
                Reg = Reg.Grid
            });
            result.Add(new RegOp<GPoint>()
            {
                Op = Op.Set,
                Reg = Reg.Index,
                Value = new GPoint() { X = 0, Y = 0 }
            });
            result.Add(new RegOp<GPoint>()
            {
                Op = Op.Push,
                Reg = Reg.Index
            });
            result.AddRange(OuterPattern.Plot());
            result.Add(new RegOp<Grid>()
            {
                Op = Op.Pop,
                Reg = Reg.Grid
            });
            return result;
        }

        /// <inheritdoc/>
        public List<PlotAction> PlotLine(long n)
        {
            return [];
        }

        /// <inheritdoc/>
        public List<PlotAction> PlotUnalligned()
        {
            return [];
        }
        #endregion Actions

        #region IEquatable<IPlot>
        /// <inheritdoc/>
        public bool Equals(IPlot? other)
            => Equals(other as TestPattern);
        #endregion IEquatable<Image>
    }
}
