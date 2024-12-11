using System.Xml.Serialization;

using As.Applications.Loggers;
using As.Applications.Plotters;
using As.Applications.Plotters.Engine;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;


using LogManager = Caliburn.Micro.LogManager;

namespace As.Applications.Data.Patterns
{
    public partial class Pattern : IPlot
    {
        static readonly ILogger Log = (ILogger)LogManager.GetLog(typeof(Pattern));

        #region Properties
        [XmlIgnore]
        public IPlot? Image
        {
            get => _image;
            set => SetNullable(ref _image, value);
        }
        IPlot? _image = null;

        /// <inheritdoc/>
        [XmlIgnore]
        public double Width => (Image == null) ? 0 : Image.Width * X.Count + X.Gap * (X.Count - 1);

        /// <inheritdoc/>
        [XmlIgnore]
        public double Height => (Image == null) ? 0 : Image.Height * Y.Count + Y.Gap * (Y.Count - 1);

        [XmlIgnore]
        public GPoint Count
        {
            get
            {
                _count.X = (Image == null) ? 0 : Image.Count.X * X.Count;
                _count.Y = (Image == null) ? 0 : Image.Count.Y * Y.Count;
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
            List<PlotAction> result = [];
            if (Image == null) return result;

            bool IsInner = (Image is not Pattern);
            var pattern = (IsInner) ? "inner" : "outer";

            var w = Image.Width + X.Gap;
            var h = Image.Height + Y.Gap;
            var c = Image.Count;
            result.Add(new RegOp<GPoint>()
            {
                Op = Op.Push,
                Reg = Reg.Position
            });
            for (var y = 0; y < Y.Count; y++)
            {
                for (var x = 0; x < X.Count; x++)
                {
                    result.Add(new Move()
                    {
                        Home = Reference.StackRelative,
                        P = new GPoint() { X = x * w, Y = y * h },
                    });
                    result.Add(new MoveIndex()
                    {
                        Home = Reference.StackRelative,
                        P = new GPoint() { X = x * c.X, Y = y * c.Y },
                    });
                    result.Add(new RegOp<GPoint>()
                    {
                        Op = Op.Push,
                        Reg = Reg.Index
                    });
                    result.Add(new SetVal() { Val = Val.Height });
                    result.Add(new SetVal() { Val = Val.Speed });
                    result.Add(new SetVal() { Val = Val.Power });
                    result.AddRange(Image.Plot());
                    result.Add(new RegOp<GPoint>()
                    {
                        Op = Op.Pop,
                        Reg = Reg.Index
                    });
                }
            }
            result.Add(new RegOp<GPoint>()
            {
                Op = Op.Pop,
                Reg = Reg.Position
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
        public virtual bool Equals(IPlot? other)
            => Equals(other as Pattern);
        #endregion IEquatable<Image>
    }
}
