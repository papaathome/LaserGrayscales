using System.Numerics;
using System.Runtime.CompilerServices;

namespace As.Applications.Validation
{
    internal static class ValidNumeric
    {
        /// <summary>
        /// Check if a value is greater (or equal) than a minimum value
        /// </summary>
        /// <typeparam name="T">Type of value and minimum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="open">true: do not alow minimum value, false: allow minimum value</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if value is greater (or equal) than the minimum, errror message otherwise</returns>
        static public string ValidateMinimum<T>(
            this T value,
            T minimum,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var result = string.Empty;
            value.TryIsValidMinimum(minimum, ref result, open, name);
            return result;
        }

        /// <summary>
        /// Check if a value is less (or equal) than a maximum value
        /// </summary>
        /// <typeparam name="T">Type of value and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="open">true: do not alow maximum value, false: allow maximum value</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if value is less (or equal) than the minimum, errror message otherwise</returns>
        static public string ValidateMaximum<T>(
            this T value,
            T maximum,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var result = string.Empty;
            value.TryIsValidMaximum(maximum, ref result, open, name);
            return result;
        }

        /// <summary>
        /// Check if value is in open range (minimum, maximum) or in closed range [minimum, maximum]
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open">true: do not alow minimum and maximum values, false: allow minimum and maximum values</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if value is in range, errror message otherwise</returns>
        static public string ValidateRange<T>(
            this T value,
            T minimum,
            T maximum,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var result = string.Empty;
            value.TryIsValidRange(minimum, maximum, ref result, open, name);
            return result;
        }

        /// <summary>
        /// Check if value is in a range where the range is independenly open or closed at either end.
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="open_minimum">true: do not alow minimum value, false: allow minimum value</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open_maximum">true: do not alow maximum value, false: allow maximum value</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if value is in range, errror message otherwise</returns>
        static public string ValidateRange<T>(
            this T value,
            T minimum, bool open_minimum,
            T maximum, bool open_maximum,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var result = string.Empty;
            value.TryIsValidRange(minimum, open_minimum, maximum, open_maximum, ref result, name);
            return result;
        }

        /// <summary>
        /// Check if value is in open range (minimum, maximum) or in closed range [minimum, maximum]
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="message">Unchanged if value is in range, errror message otherwise</param>
        /// <param name="name">Name of the value</param>
        /// <param name="open">true: do not alow minimum and maximum values, false: allow minimum and maximum values</param>
        /// <returns>True if value is in range, false otherwise</returns>
        static public bool TryIsValidRange<T>(
            this T value,
            T minimum,
            T maximum,
            ref string message,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            if (!value.TryIsValidMinimum(minimum, ref message, open, name)) return false;
            return value.TryIsValidMaximum(maximum, ref message, open, name);
        }

        /// <summary>
        /// Check if value is in a range where the range is independenly open or closed at either end.
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="open_minimum">true: do not alow minimum value, false: allow minimum value</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open_maximum">true: do not alow maximum value, false: allow maximum value</param>
        /// <param name="message">Unchanged if value is in range, errror message otherwise</param>
        /// <param name="name">Name of the value</param>
        /// <returns>True if value is in range, false otherwise</returns>
        static public bool TryIsValidRange<T>(
            this T value,
            T minimum, bool open_minimum,
            T maximum, bool open_maximum,
            ref string message,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            if (!value.TryIsValidMinimum(minimum, ref message, open_minimum, name)) return false;
            return value.TryIsValidMaximum(maximum, ref message, open_maximum, name);
        }

        /// <summary>
        /// Check if a value is greater (or equal) than a minimum value
        /// </summary>
        /// <typeparam name="T">Type of value and minimum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="message">Empty if value is greater (or equal) than the minimum, errror message otherwise</param>
        /// <param name="name">Name of the value</param>
        /// <param name="open">true: do not alow minimum value, false: allow minimum value</param>
        /// <returns>True if value is greater (or equal) than the minimum, false otherwise</returns>
        static public bool TryIsValidMinimum<T>(
            this T value,
            T minimum,
            ref string message,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var v = value.CompareTo(minimum);
            if (0 < v || !open && v == 0) return true;

            name ??= "Value";
            message = open
                ? $"{name} {value} is less than or equal to the minimum of {minimum}"
                : $"{name} {value} is less than the minimum of {minimum}";
            return false;
        }

        /// <summary>
        /// Check if a value is less (or equal) than a maximum value
        /// </summary>
        /// <typeparam name="T">Type of value and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="message">Unchanged if value is less (or equal) than the minimum, errror message otherwise</param>
        /// <param name="name">Name of the value</param>
        /// <param name="open">true: do not alow maximum value, false: allow maximum value</param>
        /// <returns>True if value is less (or equal) than the minimum, false otherwise</returns>
        static public bool TryIsValidMaximum<T>(
            this T value,
            T maximum,
            ref string message,
            bool open = false,
            [CallerMemberName] string name = "")
            where T : IComparable<T>
        {
            var v = value.CompareTo(maximum);
            if (v < 0 || !open && v == 0) return true;

            name ??= "Value";
            message = open
                ? $"{name} {value} is greater than or equal to the maximum of {maximum}"
                : $"{name} {value} is greater than the maximum of {maximum}";
            return false;
        }

        /// <summary>
        /// Check if a type is numeric
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="value">a value in type T</param>
        /// <param name="name">Name of the value</param>
        /// <returns>Empty if type is Numeric, error message otherwise</returns>
        static public string ValidateIsNumeric<T>(
            this T value,
            [CallerMemberName] string name = "")
        {
            var result = "";
            TryIsNumeric<T>(value, ref result, name);
            return result;
        }

        /// <summary>
        /// Check if a type is numeric
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <param name="value">a value in type T</param>
        /// <param name="message">Unchanged if type T is numeric, error message otherwise</param>
        /// <param name="name">Name of the value</param>
        /// <returns>True if type is numeric, false otherwise</returns>
        static public bool TryIsNumeric<T>(
            this T value,
            ref string message,
            [CallerMemberName] string name = "")
        {
            if (TryIsNumeric<T>()) return true;

            name ??= "Value";
            message = $"{name} {nameof(T)} is not a numeric type";
            return false;
        }

        /// <summary>
        /// Check if a type is numeric
        /// </summary>
        /// <typeparam name="T">Type to check</typeparam>
        /// <returns>True if type is numeric, false otherwise</returns>
        static public bool TryIsNumeric<T>()
        {
            return typeof(T)
                .GetInterfaces()
                .Any(e =>
                    e.IsGenericType &&
                    (e.GetGenericTypeDefinition() == typeof(INumber<>)));
        }
    }
}
