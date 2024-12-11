using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    public class SetVal : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(SetVal), $"{nameof(Val)}={Val}");

        [XmlElement("val")]
        public Val Val { get; set; }

        internal override bool TryWriteGcode(Context context)
        {
            return context.TrySetVal(this);
        }
    }
}
