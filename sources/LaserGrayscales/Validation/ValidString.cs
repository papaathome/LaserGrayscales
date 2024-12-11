using System.Globalization;
using System.Runtime.CompilerServices;

namespace As.Applications.Validation
{
    internal static class ValidString
    {
        /// <summary>
        /// Check if string is not empty
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateNotNullOrWhiteSpace(this string? me, [CallerMemberName] string name = "")
        {
            return (!string.IsNullOrWhiteSpace(me)) ? "" : $"{name}: null or whitespace, not accepted";
        }

        /// <summary>
        /// Check if string contains a boolean value
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateIsBooleanValue(this string? me, [CallerMemberName] string name = "")
        {
            return ((me != null) && (me.Equals("true") || me.Equals("false"))) ? "" : $"{name}: need true or false";
        }

        /// <summary>
        /// Check if string contains an integer (InvariantCulture)
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateIsIntValue(this string? me, [CallerMemberName] string name = "")
        {
            return ((me != null) && int.TryParse(me, CultureInfo.InvariantCulture, out int result)) ? "" : $"{name}: need integer value";
        }

        /// <summary>
        /// Check if string contains an integer (InvariantCulture)
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateIsIntValueOrEmpty(this string? me, [CallerMemberName] string name = "")
        {
            return ((me != null) && (string.IsNullOrEmpty(me) || int.TryParse(me, CultureInfo.InvariantCulture, out int result))) ? "" : $"{name}: need integer value";
        }

        /// <summary>
        /// Check if string contains an double floating point (InvariantCulture)
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateIsDoubleValue(this string? me, [CallerMemberName] string name = "")
        {
            return ((me != null) && double.TryParse(me, CultureInfo.InvariantCulture, out double result)) ? "" : $"{name}: need floating point value";
        }

        /// <summary>
        /// Check if string contains an double floating point (InvariantCulture)
        /// </summary>
        /// <param name="me">Value to check</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if accepted; Error message otherwise</returns>
        static public string ValidateIsDoubleValueOrEmpty(this string? me, [CallerMemberName] string name = "")
        {
            return ((me != null) && (string.IsNullOrEmpty(me) || double.TryParse(me, CultureInfo.InvariantCulture, out double result))) ? "" : $"{name}: need floating point value";
        }
    }
}
