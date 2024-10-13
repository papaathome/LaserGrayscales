using System.Xml.Serialization;

using As.Applications.Data;

namespace As.Applications.Models
{
    public class GrayScale : IEquatable<GrayScale>
    {
        /// <summary>
        /// Create new grayscale (with empty lists)
        /// </summary>
        /// <remarks>An Xml desirialiser will fill the lists by appending lines</remarks>
        public GrayScale()
        {
            Clear();
        }

        /// <summary>
        /// Create new grayscale (optional: with default settings in lists)
        /// </summary>
        /// <param name="defaults">true: fill lists with defaults; false keep lists empty</param>
        public GrayScale(bool defaults)
        {
            if (!defaults) Clear();
        }

        internal GrayScale(GrayScale other)
        {
            Y = new Scale(other.Y);
            X = new Scale(other.X);
            GroupCount = other.GroupCount;
            GroupGap = other.GroupGap;
            Mode = other.Mode;
            Intro = other.Intro;
            Header = other.Header;
            Footer = other.Footer;
        }

        void Clear()
        {
            // todo: clear any list with default content.
            // otherwise it will grow without end by adding default values on reading content.

            Intro = string.Empty;
            Header = string.Empty;
            Footer = string.Empty;
        }

        #region IEquatable
        public bool Equals(GrayScale? other)
        {
            if (other == null) return false;
            return
                Y.Equals(other.Y) &&
                X.Equals(other.X) &&
                GroupCount.Equals(other.GroupCount) &&
                GroupGap.Equals(other.GroupGap) &&
                Mode.Equals(other.Mode) &&
                Intro.Equals(other.Intro) &&
                Header.Equals(other.Header) &&
                Footer.Equals(other.Footer);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not GrayScale)) return false;
            return Equals(obj as GrayScale);
        }

        public override int GetHashCode()
        {
            int result = 0x3c3c3c3c;
            result ^= Y.GetHashCode();
            result ^= X.GetHashCode();
            result ^= GroupCount.GetHashCode();
            result ^= GroupGap.GetHashCode();
            result ^= Mode.GetHashCode();
            result ^= Intro.GetHashCode();
            result ^= Header.GetHashCode();
            result ^= Footer.GetHashCode();
            return result;
        }
        #endregion IEquatable

        #region Data
        [XmlElement(ElementName = "y_scale")]
        public Scale Y { get; set; } = new Scale();

        [XmlElement(ElementName = "x_scale")]
        public Scale X { get; set; } = new Scale();

        [XmlElement(ElementName = "x_group_count")]
        public int GroupCount { get; set; } = 8;

        [XmlElement(ElementName = "x_group_gap")]
        public double GroupGap { get; set; } = 0.8;

        [XmlElement(ElementName = "x_group_mode")]
        public Mode Mode { get; set; } = Mode.Power;

        [XmlArray("intro"), XmlArrayItem(typeof(string), ElementName = "line")]
        public List<string> XmlIntro { get; set; } =
@"( This file will test the effect of your laser at power levels 1 through 255 at speeds ranging from 1 mm/s to 70 mm/s or 60 mm/min to 4200 mm/min. )
( This is a good way to see how your laser will react to different materials at different speeds & power levels. )

(   >>> ALWAYS WEAR PROPER EYE PROTECTION WHEN WORKING WITH LASERS <<<   )

( This file is generated for use with Stepcraft machines using UCCNC. You may be able to use it on other machines. )
( Calling a subroutine is done with 'M98 P100 L1' )
( M98, subroutine call, P is the subroutine label, L is the times of repeating the call. )
( M99, return from subroutine. )

( Controling the laser is done with 'M10 Qxxx' or 'M11' )
( M3, Laser operation enabled )
( M5, Laser operation disabled )
( M10, Laser on with parameter Q for intensity with xxx in the range of 0 to 255 which is 0 to 100 percent. )
( M11, Laser off. )".ToLst();

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
        public List<string> XmlHeader { get; set; } =
@"( Header start )
G90    ( absolute positions )
M3     ( Laser operation enabled )
M10 Q0 ( Laser on, power 0 )
( Header end )".ToLst();

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
        public List<string> XmlFooter { get; set; } =
@"( Footer )
M11    ( Laser off )
M5     ( Laser operation disabled )
G0 X0 Y0 Z0 ( go home )
M30    ( Program end, rewind )
( Footer end )".ToLst();

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
        #endregion Data
    }
}
