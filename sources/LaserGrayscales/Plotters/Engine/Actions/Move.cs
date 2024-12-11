using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    /// <summary>
    /// Move to position without plot actions
    /// </summary>
    public class Move : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(Move), $"{nameof(Home)}={Home}; {nameof(P)}={P}");

        [XmlAttribute("home")]
        public Reference Home { get; set; } = Reference.Absolute;

        [XmlElement("p")]
        public GPoint P { get; set; } = new GPoint();

        internal override Comment? Info { get; set; } = null;

        internal override bool TryWriteGcode(Context context)
        {
            if (!context.TryGoto(P, Home)) return false;
            if (context.Verbose)
            {
                if (Info == null) Info = new Comment() { Message = $"{this}" };
                else Info.Message = $"{this}; {Info.Message}";
            }

            context.WriteLaserIdle();
            context.Write($"G0");
            context.X.WriteGcode(context);
            context.Y.WriteGcode(context);
            context.F0.WriteGcode(context);

            base.TryWriteGcode(context);
            return true;
        }
    }
}
