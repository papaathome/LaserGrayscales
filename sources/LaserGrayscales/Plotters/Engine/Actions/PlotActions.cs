using System.Xml.Serialization;

using As.Applications.IO;

namespace As.Applications.Plotters.Engine.Actions
{
    public class PlotActions
    {
        #region Xml Serialization
        /// <summary>
        /// Read plot actions from file
        /// </summary>
        /// <param name="path">Path to file with content</param>
        /// <param name="result">Reference to content location</param>
        /// <param name="create_if_missing">True: create file if it is missing; False: skip reading if it is missing and return no result</param>
        /// <returns>True if a content is available (read or created), false if no content is available</returns>
        public static bool ReadXml(
            string path,
            out List<PlotAction> result,
            bool create_if_missing = false)
        {
            if (XmlStream.Read(path, out PlotActions r, create_if_missing: false, noexcept: false))
            {
                result = r.Actions;
                return true;
            }
            result = new List<PlotAction>();
            return false;
        }

        /// <summary>
        /// Write test settings to file.
        /// </summary>
        /// <param name="path">Path to file with setting content</param>
        /// <param name="value">Reference to content</param>
        /// <returns>True if content was writen; False otherwise</returns>
        public static bool WriteXml(
            string path,
            List<PlotAction> value)
        {
            var v = new PlotActions()
            {
                Actions = value
            };
            return XmlStream.Write(path, v, create_backup: false, noexcept: false);
        }
        #endregion XML Serialization

        [XmlElement("action")]
        public List<PlotAction> Actions { get; set; } = new List<PlotAction>();
    }
}
