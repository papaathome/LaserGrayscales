using System.Xml.Serialization;

namespace As.Applications.Plotters.Engine.Actions
{
    public class AddGcode : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(AddGcode), $"{nameof(GCode)}={GCode}");

        [XmlAttribute(AttributeName = "gcode")]
        public string GCode { get; set; } = "";

        [XmlIgnore]
        internal override Comment? Info { get; set; } = null;

        internal override bool TryWriteGcode(Context context)
        {
            if (string.IsNullOrEmpty(GCode)) return false;
            if (context.Verbose)
            {
                if (Info == null) Info = new Comment() { Message = $"{this}" };
                else Info.Message = $"{this}; {Info.Message}";
            }

            context.Write(GCode);
            base.TryWriteGcode(context);
            return true;
        }
    }
}
