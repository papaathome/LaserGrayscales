using System.Xml.Serialization;

using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters.Engine.Actions
{
    public class RegOp<T> : PlotAction where T : class, IEquatable<T>
    {
        public override string ToString()
        {
            return (Value == null)
                ? Applications.Data.ToString.Format(nameof(RegOp<T>), $"{nameof(Reg)}={Reg}, {nameof(Op)}={Op}")
                : Applications.Data.ToString.Format(nameof(RegOp<T>), $"{nameof(Reg)}={Reg}, {nameof(Op)}={Op}, {nameof(Value)}={Value}");
        }

        [XmlElement("reg")]
        public Reg Reg { get; set; }

        [XmlElement("op")]
        public Op Op { get; set; }

        [XmlElement("value")]
        public T? Value { get; set; }
        public bool ShouldSerializeValue() => Value != null;

        internal override bool TryWriteGcode(Context context)
        {
            if (!context.TryRegOp(this)) return false;
            return base.TryWriteGcode(context);
        }
    }
}
