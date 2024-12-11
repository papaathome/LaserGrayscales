using System.Xml.Serialization;

using As.Applications.Validation;

namespace As.Applications.Config
{
    public class MachineSettings : SettingsBase
    {
        protected override void Initialise()
        {
            SetValue(
                nameof(Name),
                "Stepcraft D3/600",
                "Target CNC machine.",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(XMinimum),
                "0.00",
                "X-axis minimum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(XMaximum),
                "417.00",
                "X-axis maximum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(YMinimum),
                "0.00",
                "Y-axis minimum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(YMaximum),
                "596.00",
                "Y-axis maximum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(ZMinimum),
                "-132.00",
                "Z-axis minimum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(ZMaximum),
                "0.00",
                "Z-axis maximum value (mm) in machine coordinates.",
                ValidString.ValidateIsDoubleValue);

            SetValue(
                nameof(G0FMinimum),
                "1",
                "G0 F speed minimum value (mm/sec).",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(G0FMaximum),
                "70",
                "G0 F speed maximum value (mm/sec).",
                ValidString.ValidateIsIntValue);

            SetValue(
                nameof(G1FMinimum),
                "1",
                "G1 F speed minimum value (mm/sec).",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(G1FMaximum),
                "70",
                "G1 F speed maximum value (mm/sec).",
                ValidString.ValidateNotNullOrWhiteSpace);

            SetValue(
                nameof(PowerMinimum),
                "0",
                "Laser power minimum value (parts of 100%/255).",
                ValidString.ValidateIsIntValue);

            SetValue(
                nameof(PowerMaximum),
                "255",
                "Laser poser maximum value (parts of 100%/255).",
                ValidString.ValidateIsIntValue);
        }

        #region Properties
        /// <summary>
        /// Optional machine name using these settings.
        /// </summary>
        [XmlIgnore]
        public string Name => GetString() ?? "";

        [XmlIgnore]
        public double XMinimum => Get<double>();

        [XmlIgnore]
        public double XMaximum => Get<double>();

        [XmlIgnore]
        public double YMinimum => Get<double>();

        [XmlIgnore]
        public double YMaximum => Get<double>();

        [XmlIgnore]
        public double ZMinimum => Get<double>();

        [XmlIgnore]
        public double ZMaximum => Get<double>();

        [XmlIgnore]
        public double G0FMinimum => Get<double>();

        [XmlIgnore]
        public double G0FMaximum => Get<double>();

        [XmlIgnore]
        public double G1FMinimum => Get<double>();

        [XmlIgnore]
        public double G1FMaximum => Get<double>();

        [XmlIgnore]
        public int PowerMinimum => Get<int>();

        [XmlIgnore]
        public int PowerMaximum => Get<int>();
        #endregion Properties
    }
}
