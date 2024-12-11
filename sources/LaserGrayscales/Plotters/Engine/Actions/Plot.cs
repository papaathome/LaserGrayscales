using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    /// <summary>
    /// Move to position with plot action active.
    /// </summary>
    public class Plot : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(Plot), $"{nameof(Home)}={Home}; {nameof(P)}={P}");

        [XmlAttribute("home")]
        public Reference Home { get; set; }

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

            context.WriteLaserOn();
            context.Write($"G1");
            context.X.WriteGcode(context);
            context.Y.WriteGcode(context);
            context.Z.WriteGcode(context);
            context.F.WriteGcode(context);

            base.TryWriteGcode(context);
            return true;
        }
    }
}
