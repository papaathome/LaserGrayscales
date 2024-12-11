namespace As.Applications.Plotters.Engine.Actions
{
    /// <summary>
    /// Move grid to (X0, Y0);
    /// </summary>
    public class MoveGrid : PlotAction
    {
        public override string ToString() => Applications.Data.ToString.Format(nameof(MoveGrid));

        internal override bool TryWriteGcode(Context context)
        {
            return false;
        }
    }
}
