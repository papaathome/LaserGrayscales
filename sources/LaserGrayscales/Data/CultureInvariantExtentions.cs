using System.Globalization;

namespace As.Applications.Data
{
    static internal class CultureInvariantExtentions
    {
        /// <summary>
        /// Convert int to string, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <returns>converted value</returns>
        static public string Ci(this int me)
        {
            return me.ToString("0", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert string to int, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <param name="target_type">value used for type information only</param>
        /// <returns>converted value</returns>
        static public int Ci(this string me, int target_type)
        {
            return int.Parse(me, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Try convert string to int, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <param name="value">Conversion result</param>
        /// <returns>True if value is valid, false otherwise</returns>
        static public bool Ci(this string me, out int value)
        {
            return int.TryParse(me, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Convert double to string, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <returns>converted value</returns>
        static public string Ci(this double me)
        {
            return me.ToString("0.###", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert string to double, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <param name="target_type">value used for type information only</param>
        /// <returns>converted value</returns>
        static public double Ci(this string me, double target_type)
        {
            return double.Parse(me, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Try convert string to double, culture invariant
        /// </summary>
        /// <param name="me">value to convert</param>
        /// <param name="value">Conversion result</param>
        /// <returns>True if value is valid, false otherwise</returns>
        static public bool Ci(this string me, out double value)
        {
            return double.TryParse(me, CultureInfo.InvariantCulture, out value);
        }
    }
}
