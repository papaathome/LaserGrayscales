﻿namespace As.Applications.Data
{
    internal static class ValidatingExtensions
    {
        /// <summary>
        /// Check if a value is greater (or equal) than a minimum value
        /// </summary>
        /// <typeparam name="T">Type of value and minimum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="open_interval">true: do not alow minimum value, false: allow minimum value</param>
        /// <returns>Empty if value is greater (or equal) than the minimum, errror message otherwise</returns>
        static public string ValidateMinimum<T>(
            this T value,
            T minimum,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var result = string.Empty;
            TryIsValidMinimum<T>(value, minimum, ref result, open_interval);
            return result;
        }

        /// <summary>
        /// Check if a value is less (or equal) than a maximum value
        /// </summary>
        /// <typeparam name="T">Type of value and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="open_interval">true: do not alow maximum value, false: allow maximum value</param>
        /// <returns>Empty if value is less (or equal) than the minimum, errror message otherwise</returns>
        static public string ValidateMaximum<T>(
            this T value,
            T maximum,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var result = string.Empty;
            TryIsValidMaximum<T>(value, maximum, ref result, open_interval);
            return result;
        }

        /// <summary>
        /// Check if value is in open range (minimum, maximum) or in closed range [minimum, maximum]
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open_interval">true: do not alow minimum and maximum values, false: allow minimum and maximum values</param>
        /// <returns>Empty if value is in range, errror message otherwise</returns>
        static public string ValidateRange<T>(
            this T value,
            T minimum,
            T maximum,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var result = string.Empty;
            TryIsValidRange<T>(value, minimum, maximum, ref result, open_interval);
            return result;
        }

        /// <summary>
        /// Check if value is in a range where the range is independenly open or closed at either end.
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="open_minimum_interval">true: do not alow minimum value, false: allow minimum value</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open_maximum_interval">true: do not alow maximum value, false: allow maximum value</param>
        /// <returns>Empty if value is in range, errror message otherwise</returns>
        static public string ValidateRange<T>(
            this T value,
            T minimum, bool open_minimum_interval,
            T maximum, bool open_maximum_interval)
            where T : IComparable<T>
        {
            var result = string.Empty;
            TryIsValidRange<T>(value, minimum, open_minimum_interval, maximum, open_maximum_interval, ref result);
            return result;
        }

        /// <summary>
        /// Check if value is in open range (minimum, maximum) or in closed range [minimum, maximum]
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="message">Empty if value is in range, errror message otherwise</param>
        /// <param name="open_interval">true: do not alow minimum and maximum values, false: allow minimum and maximum values</param>
        /// <returns>True if value is in range, false otherwise</returns>
        static public bool TryIsValidRange<T>(
            this T value,
            T minimum,
            T maximum,
            ref string message,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var result = TryIsValidMinimum(value, minimum, ref message, open_interval);
            if (result) result = TryIsValidMaximum(value, maximum, ref message, open_interval);
            return result;
        }

        /// <summary>
        /// Check if value is in a range where the range is independenly open or closed at either end.
        /// </summary>
        /// <typeparam name="T">Type of value, minimum and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value of the range</param>
        /// <param name="open_minimum_interval">true: do not alow minimum value, false: allow minimum value</param>
        /// <param name="maximum">Maximum value of the range</param>
        /// <param name="open_maximum_interval">true: do not alow maximum value, false: allow maximum value</param>
        /// <param name="message">Empty if value is in range, errror message otherwise</param>
        /// <returns>True if value is in range, false otherwise</returns>
        static public bool TryIsValidRange<T>(
            this T value,
            T minimum, bool open_minimum_interval,
            T maximum, bool open_maximum_interval,
            ref string message)
            where T : IComparable<T>
        {
            var result = TryIsValidMinimum(value, minimum, ref message, open_minimum_interval);
            if (result) result = TryIsValidMaximum(value, maximum, ref message, open_maximum_interval);
            return result;
        }

        /// <summary>
        /// Check if a value is greater (or equal) than a minimum value
        /// </summary>
        /// <typeparam name="T">Type of value and minimum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="message">Empty if value is greater (or equal) than the minimum, errror message otherwise</param>
        /// <param name="open_interval">true: do not alow minimum value, false: allow minimum value</param>
        /// <returns>True if value is greater (or equal) than the minimum, false otherwise</returns>
        static public bool TryIsValidMinimum<T>(
            this T value,
            T minimum,
            ref string message,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var v = value.CompareTo(minimum);
            var result = (0 < v) || !open_interval && (v == 0);

            if (!result)
            {
                message = (open_interval)
                    ? $"Value {value} is less than or equal to the minimum of {minimum}"
                    : $"Value {value} is less than the minimum of {minimum}";
            }
            return result;
        }

        /// <summary>
        /// Check if a value is less (or equal) than a maximum value
        /// </summary>
        /// <typeparam name="T">Type of value and maximum</typeparam>
        /// <param name="value">Value to check</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="message">Empty if value is less (or equal) than the minimum, errror message otherwise</param>
        /// <param name="open_interval">true: do not alow maximum value, false: allow maximum value</param>
        /// <returns>True if value is less (or equal) than the minimum, false otherwise</returns>
        static public bool TryIsValidMaximum<T>(
            this T value,
            T maximum,
            ref string message,
            bool open_interval = false)
            where T : IComparable<T>
        {
            var v = value.CompareTo(maximum);
            var result = (v < 0) || !open_interval && (v == 0);

            if (!result)
            {
                message = (open_interval)
                    ? $"Value {value} is greater than or equal to the maximum of {maximum}"
                    : $"Value {value} is greater than the maximum of {maximum}";
            }
            return result;
        }
    }
}
