using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    [XmlInclude(typeof(Comment))]
    [XmlInclude(typeof(Move))]
    [XmlInclude(typeof(Plot))]
    [XmlInclude(typeof(MoveIndex))]
    [XmlInclude(typeof(MoveGrid))]
    [XmlInclude(typeof(AddGcode))]
    [XmlInclude(typeof(RegOp<GPoint>))]
    [XmlInclude(typeof(RegOp<Grid>))]
    [XmlInclude(typeof(SetVal))]
    public abstract class PlotAction
    {
        internal virtual Comment? Info
        {
            get => null;
            set => throw new NotImplementedException();
        }

        internal virtual bool TryWriteGcode(Context context)
        {
            if (Info == null) return false;
            Info.Column = context.GCode.InfoColumn;

            context.Write(" ");
            return Info.TryWriteGcode(context);
        }
    }
}
