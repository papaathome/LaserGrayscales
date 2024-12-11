using System.IO;

using As.Applications.Config;
using As.Applications.Data.Patterns;
using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine
{
    internal class Context
    {
        #region Properties
        public GCodeSettings GCode => Settings.App.GCode;

        public DateTime Now { get; } = DateTime.Now;

        internal bool Verbose { get; set; } = true;

        internal bool Debug { get; set; } = false;

        internal Axis XLimits { get; set; } = new Axis();

        internal Axis YLimits { get; set; } = new Axis();

        internal Axis MachineLimits { get; set; } = new Axis();

        internal GPoint IndexMax { get; set; } = new GPoint();

        public Register<GPoint> Index { get; set; } = new Register<GPoint>()
        {
            Value = new GPoint() { X = 0, Y = 0 }
        };

        public Register<Grid> Grid { get; set; } = new Register<Grid>()
        {
            Value = new Grid()
            {
                P0 = new GPoint() { X = 0, Y = 0 },
                e = new GPoint() { X = 1, Y = 1 }
            }
        };

        public Register X { get; set; } = new Register() { GName = "X", Value =0.0 };

        public Register Y { get; set; } = new Register() { GName = "Y", Value = 0.0 };

        public GValue Z { get; set; } = new GValue() { GName = "Z" };

        public GValue F0 { get; set; } = new GValue() { GName = "F", Fractional = false };

        public GValue F { get; set; } = new GValue() { GName = "F", Fractional = false };

        public GValue S { get; set; } = new GValue() { GName = "S", Fractional = false };

        bool SZero = false;
        #endregion Properties

        #region Actions
        public bool TryGoto(GPoint P, Reference home)
        {
            // context is abs.
            GPoint p;
            switch (home)
            {
                case Reference.Absolute:
                    if (X.Value.Equals(P.X) && Y.Value.Equals(P.Y)) return false;
                    X.Value = P.X;
                    Y.Value = P.Y;
                    break;
                case Reference.Relative:
                    if (P.IsZero) return false;
                    X.Value += P.X;
                    Y.Value += P.Y;
                    break;
                case Reference.GridRelative:
                    if (Grid.Count == 0) throw new Exception("Plot: Grid Stack empty");
                    p = Grid.Peek().P0 + P;
                    if (X.Value.Equals(p.X) && Y.Value.Equals(p.Y)) return false;
                    X.Value = p.X;
                    Y.Value = p.Y;
                    break;
                case Reference.StackRelative:
                    if (X.Count == 0) throw new Exception("Plot: X Stack empty");
                    if (Y.Count == 0) throw new Exception("Plot: Y Stack empty");
                    p = new GPoint() { X = X.Peek(), Y = Y.Peek() } + P;
                    if (X.Value.Equals(p.X) && Y.Value.Equals(p.Y)) return false;
                    X.Value = p.X;
                    Y.Value = p.Y;
                    break;
                default:
                    throw new Exception($"Plot: Home not recognised; Home={home}");
            }
            return true;
        }

        public bool TryGotoIndex(GPoint P, Reference home)
        {
            // context is abs.
            switch (home)
            {
                case Reference.Absolute:
                    if (Index.Equals(P)) return false;
                    Index.Value = P;
                    break;
                case Reference.Relative:
                    if (P.IsZero) return false;
                    Index.Value += P;
                    break;
                case Reference.StackRelative:
                    if (Index.Count == 0) throw new Exception("Plot: GotoIndex Stack empty");
                    var p = Index.Peek() + P;
                    if (Index.Equals(p)) return false;
                    Index.Value = p;
                    break;
                case Reference.GridRelative:
                    // invalid op
                    throw new Exception($"Plot: GotoIndex invalid reference; Home={home}");
                default:
                    throw new Exception($"Plot: GotoIndex reference not recognised; Home={home}");
            }
            return true;
        }

        public bool TryRegOp<T>(RegOp<T> regop) where T : class, IEquatable<T>
        {
            if (regop is RegOp<GPoint>) return TryRegOp(regop as RegOp<GPoint>);
            if (regop is RegOp<Grid>) return TryRegOp(regop as RegOp<Grid>);
            throw new Exception($"Plot: {nameof(RegOp<T>)} regop not regocnised");
        }

        bool TryRegOp(RegOp<GPoint>? regop)
        {
            if (regop == null) throw new Exception($"Plot: {nameof(RegOp<GPoint>)} regop is null");
            switch (regop.Reg)
            {
                case Reg.Position:
                case Reg.Index:
                    break;
                default:
                    throw new Exception($"Plot: {nameof(RegOp<GPoint>)} on unmatching register; {nameof(regop.Reg)}={regop.Reg}");
            }

            switch (regop.Op)
            {
                case Op.Set:
                    if (regop.Value == null) throw new Exception($"Plot: {nameof(RegOp<GPoint>)} value missing; {nameof(regop.Op)}={regop.Op}");
                    switch (regop.Reg)
                    {
                        case Reg.Position:
                            X.Value = regop.Value.X;
                            Y.Value = regop.Value.Y;
                            break;
                        case Reg.Index:
                            Index.Value = regop.Value;
                            break;
                    }
                    break;
                case Op.Push:
                    switch (regop.Reg)
                    {
                        case Reg.Position:
                            X.Push();
                            Y.Push();
                            break;
                        case Reg.Index:
                            Index.Push();
                            break;
                    }
                    break;
                case Op.Pop:
                    switch (regop.Reg)
                    {
                        case Reg.Position:
                            X.Pop();
                            Y.Pop();
                            break;
                        case Reg.Index:
                            Index.Pop();
                            break;
                    }
                    break;
                default:
                    throw new Exception($"Plot: {nameof(RegOp<Grid>)} operation not recognised; {nameof(regop.Op)}={regop.Op}");
            }
            return true;
        }

        bool TryRegOp(RegOp<Grid>? regop)
        {
            if (regop == null) throw new Exception($"Plot: {nameof(RegOp<Grid>)} regop is null");
            if (regop.Reg != Reg.Grid) throw new Exception($"Plot: {nameof(RegOp<Grid>)} on unmatching register; {nameof(regop.Reg)}={regop.Reg}");

            switch (regop.Op)
            {
                case Op.Set:
                    if (regop.Value == null) throw new Exception($"Plot: {nameof(RegOp<Grid>)} value missing; {nameof(regop.Op)}={regop.Op}");
                    Grid.Value = regop.Value;
                    break;
                case Op.Push:
                    Grid.Push();
                    break;
                case Op.Pop:
                    Grid.Pop();
                    break;
                default:
                    throw new Exception($"Plot: {nameof(RegOp<Grid>)} operation not recognised; {nameof(regop.Op)}={regop.Op}");
            }
            return true;
        }

        public bool TrySetVal(SetVal setval)
        {
            switch (setval.Val)
            {
                case Val.Height:
                    var h = XLimits.ZHeight.Value(Index.Value.X, IndexMax.X);
                    h += YLimits.ZHeight.Value(Index.Value.Y, IndexMax.Y);
                    Z.Value = MachineLimits.ZHeight.Clip(h);
                    break;
                case Val.SpeedAtG0:
                    throw new Exception($"Plot: Can not set value; {nameof(setval.Val)}={setval.Val}");
                case Val.Speed:
                    var f = XLimits.Speed.Value(Index.Value.X, IndexMax.X);
                    f += YLimits.Speed.Value(Index.Value.Y, IndexMax.Y);
                    F.Value = MachineLimits.Speed.Clip(f);
                    break;
                case Val.Power:
                    var q = XLimits.Power.Value((int)Index.Value.X, (int)IndexMax.X);
                    q += YLimits.Power.Value((int)Index.Value.Y, (int)IndexMax.Y);
                    S.Value = MachineLimits.Power.Clip(q);
                    break;
                default:
                    throw new Exception($"Plot: Value name not recognised; {nameof(setval.Val)}={setval.Val}");
            }
            return false;
        }
        #endregion Actions

        #region Stream GCode, track current column
        StreamWriter? stream;

        int stream_column = 0;

        public void Open(string path)
        {
            if (stream != null) throw new Exception("Open: already open");
            stream = new StreamWriter(path);
            if (stream == null) throw new Exception($"Open: open failed; path=\"{path}\"");
            stream_column = 0;
        }

        public void Close()
        {
            stream?.Close();
            stream?.Dispose();
            stream = null;
        }

        public bool WriteLaserOn()
        {
            if (!S.Changed) return false;

            var to_idle = (S.Value == 0);
            if (to_idle && SZero)
            {
                S.Reset();
                return false;
            }

            Write(GCode.LaserOn);
            S.WriteGcode(this);
            SZero = (S.Value == 0);

            WriteLine();
            return false;
        }

        public bool WriteLaserIdle()
        {
            if (SZero) return false;

            Write(GCode.LaserOn);
            S.WriteGcode(this, 0);
            SZero = true;

            WriteLine();
            return false;
        }

        public void Write(string content, int column = 0)
        {
            WriteToColumn(column);
            stream?.Write(content);
            stream_column += content.Length;
        }

        public void WriteLine(string content, int column = 0)
        {
            if (0 < column) WriteToColumn(column);
            stream?.WriteLine(content);
            stream_column = 0;
        }

        public void WriteLine()
        {
            stream?.WriteLine();
            stream_column = 0;
        }

        public void WriteLineComment(string msg)
        {
            if (0 < stream_column) WriteToColumn(GCode.InfoColumn);
            stream?.Write($"{GCode.CommentOpen} {msg}");
            if (GCode.TryGetCommentClose(out string comment_close)) stream?.Write($" {comment_close}");
            stream?.WriteLine();
            stream_column = 0;
        }

        void WriteToColumn(int column)
        {
            if (stream_column < column)
            {
                stream?.Write(new string(' ', column - stream_column));
                stream_column = column;
            }
        }
        #endregion Stream GCode, track current column
    }
}
