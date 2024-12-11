using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    public class MoveIndex : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(MoveIndex), $"{nameof(Home)}={Home}; {nameof(P)}={P}");

        [XmlAttribute("home")]
        public Reference Home { get; set; } = Reference.Absolute;

        [XmlElement("p")]
        public GPoint P { get; set; } = new GPoint();

        internal override bool TryWriteGcode(Context context)
        {
            if (!context.TryGotoIndex(P, Home)) return false;
            return false;
        }
    }
}
