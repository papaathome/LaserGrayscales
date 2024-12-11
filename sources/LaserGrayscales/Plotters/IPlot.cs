using As.Applications.Plotters.Engine.Actions;
using As.Applications.Plotters.Engine.Data;

namespace As.Applications.Plotters
{
    public interface IPlot : IEquatable<IPlot?>
    {
        #region Properties
        /// <summary>
        /// Image width on X-axis
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Image height on Y-axis
        /// </summary>
        double Height { get; }

        GPoint Count { get; }
        #endregion Properties

        #region Actions
        /// <summary>
        /// Draw bitmap image.
        /// </summary>
        void Draw();

        /// <summary>
        /// Plot all of the GCode/image, generate calls to subroutines if applicable.
        /// </summary>
        List<PlotAction> Plot();

        /// <summary>
        /// Plot single line
        /// </summary>
        /// <param name="n"></param>
        /// <returns>List op plot actions</returns>
        List<PlotAction> PlotLine(long n);

        /// <summary>
        /// Plot all unalligned parts of the image.
        /// </summary>
        /// <returns>List op plot actions</returns>
        List<PlotAction> PlotUnalligned();
        #endregion Actions
    }
}
