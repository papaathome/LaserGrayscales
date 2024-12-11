using System.Xml.Serialization;

namespace As.Applications.Plotters.Engine.Actions
{
    /// <summary>
    /// Add a comment line, if column = 0 and msg is empty add a blanc line.
    /// </summary>
    public class Comment : PlotAction
    {
        internal static bool WriteGcode(Context context, int column = 0, string msg = "", bool newline = true)
        {
            if (msg == null) return false;
            if ((column == 0) && string.IsNullOrWhiteSpace(msg))
            {
                context.WriteLine();
                return false;
            }

            context.Write($"{context.GCode.CommentOpen} {msg}", column);
            if (context.GCode.TryGetCommentClose(out string c)) context.Write($" {c}");
            else newline = true;
            if (newline) context.WriteLine();
            return !newline;
        }

        public override string ToString() => Applications.Data.ToString.Format(nameof(Info), $"{nameof(Message)}=\"{Message}\"");

        [XmlAttribute("comment")]
        public string Message { get; set; } = "";

        [XmlIgnore]
        public int Column { get; set; } = 0;

        internal override bool TryWriteGcode(Context context)
            => WriteGcode(context, Column, Message, newline: false);
    }
}
