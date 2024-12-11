using System.Xml.Serialization;

using As.Applications.Converters;
using As.Applications.Validation;

namespace As.Applications.Config
{
    [XmlRoot("gcode_settings")]
    public class GCodeSettings : SettingsBase
    {
        public GCodeSettings() { } /* required for XML streaming */

        public GCodeSettings(bool set_defaults)
        {
            if (set_defaults)
            {
                XmlIntro =
@"( This file will test the effect of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min. )
( This is a good way to see how your laser will react to different materials at different speeds & power levels. )

(   >>> ALWAYS WEAR PROPER EYE PROTECTION WHEN WORKING WITH LASERS <<<   )

( Metric: for all GCode lengths in 'mm' and speeds in 'mm/min' )

( This file is generated for use with Stepcraft machines using UCCNC. You may be able to use it on other machines. )
( Controling the laser is done with 'M10 Qxxx' or 'M11' )
( M3, Laser operation enabled )
( M5, Laser operation disabled )
( M10, Laser on with parameter Q for intensity with xxx in the range of 0 to 255 which is 0 to 100 percent. )
( M11, Laser off. )".ToLst();

                XmlHeader =
@"( Header start )
( Header end )".ToLst(); ;

                XmlFooter =
@"( Footer )
G0 X0 Y0 Z0 ( go home )
M30    ( Program end, rewind )
( Footer end )".ToLst();
            }
        }

        override protected void Initialise()
        {
            SetValue(
                nameof(Interpreter),
                "UCCNC",
                "Target GCode iterpreter reading and processing.",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(CommentOpen),
                "(",
                "Comment open (Control out).",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(CommentClose),
                ")",
                "Comment close (Control in).");

            SetValue(
                nameof(FormatFloat),
                "0.###",
                "Format string for floating point numbers.");

            SetValue(
                nameof(InfoColumn),
                "25",
                "Column to start comment on lines with gcode(s).",
                ValidString.ValidateIsIntValueOrEmpty);

            SetValue(
                nameof(AbsolutePostions),
                "G90",
                "Using absolute dimension input.",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(LaserEnable),
                "M3",
                "Enable laser equipment."
                );

            SetValue(
                nameof(LaserDisable),
                "M5",
                "Disable laser equipment.",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(LaserOn),
                "M10",
                "Switch laser on",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(LaserOff),
                "M11",
                "Switch laser off",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(LaserPower),
                "Q",
                "Set laster power (in range of 0 to 255 of 100%/255 parts)",
                ValidString.ValidateNotNullOrWhiteSpace);
        }

        #region Properties
        /// <summary>
        /// Optional name of GCode consumer reading the GCode.
        /// </summary>
        [XmlIgnore]
        public string Interpreter => GetString() ?? "";

        public bool TryGetConsumer(out string consumer)
        {
            consumer = Interpreter;
            return !string.IsNullOrWhiteSpace(consumer);
        }

        /// <summary>
        /// Comment open token
        /// </summary>
        [XmlIgnore]
        public string CommentOpen => GetString() ?? "";

        /// <summary>
        /// Comment close token, if empty then EOL is used as comment close token.
        /// </summary>
        [XmlIgnore]
        public string CommentClose => GetString() ?? "";

        public bool TryGetCommentClose(out string comment_close)
        {
            comment_close = CommentClose;
            return !string.IsNullOrWhiteSpace(comment_close);
        }

        /// <summary>
        /// Format of floating point numbers.
        /// </summary>
        [XmlIgnore]
        public string FormatFloat => GetString() ?? "";

        /// <summary>
        /// Preferred column for additional information comments (if available)
        /// </summary>
        [XmlIgnore]
        public int InfoColumn => Get<int>();

        /// <summary>
        /// Enable absolute positions for GCode.
        /// </summary>
        [XmlIgnore]
        public string AbsolutePostions => GetString() ?? "";

        /// <summary>
        /// Enable laser GCode, if empty no GCode to enable the laser is generated. Enable laser is done prior to all laser actions.
        /// </summary>
        [XmlIgnore]
        public string LaserEnable => GetString() ?? "";

        /// <summary>
        /// Disable laser GCode, if empty no GCode to disable the laser is generated. Disable laser is done as last after all laser actions.
        /// </summary>
        [XmlIgnore]
        public string LaserDisable => GetString() ?? "";

        /// <summary>
        /// Switch on laser followed by LaserPower GCode to swich on the laser at some power level
        /// </summary>
        [XmlIgnore]
        public string LaserOn => GetString() ?? "";

        /// <summary>
        /// Switch off laser
        /// </summary>
        [XmlIgnore]
        public string LaserOff => GetString() ?? "";

        /// <summary>
        /// Set laser power level GCode immediately followed by a numeric value for the power level.
        /// </summary>
        [XmlIgnore]
        public string LaserPower => GetString() ?? "";

        [XmlArray("intro"), XmlArrayItem(typeof(string), ElementName = "line")]
        public List<string> XmlIntro { get; set; } = [];

        [XmlIgnore]
        public string Intro
        {
            get { return XmlIntro.ToStr(); }
            set
            {
                if (value == null) XmlIntro.Clear();
                else XmlIntro = value.ToLst();
            }
        }

        [XmlArray("header"), XmlArrayItem(typeof(string), ElementName = "line")]
        public List<string> XmlHeader { get; set; } = [];

        [XmlIgnore]
        public string Header
        {
            get { return XmlHeader.ToStr(); }
            set
            {
                if (value == null) XmlHeader.Clear();
                else XmlHeader = value.ToLst();
            }
        }

        [XmlArray("footer"), XmlArrayItem(typeof(string), ElementName = "line")]
        public List<string> XmlFooter { get; set; } = [];

        [XmlIgnore]
        public string Footer
        {
            get { return XmlFooter.ToStr(); }
            set
            {
                if (value == null) XmlFooter.Clear();
                else XmlFooter = value.ToLst();
            }
        }
        #endregion Properties
    }
}
