using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace As.Applications.Validation
{
    public class DataErrorInfo(object parent) : IDataErrorInfo
    {
        readonly object _parent = parent;

        public string Name { get; set; } = "";

        readonly Dictionary<string, string> _named_errors = [];

        readonly StringBuilder _sb = new();

        public event DataErrorInfoChangedEventHandler? DataErrorInfoChanged;

        /// <inheritdoc/>
        public string Error
        {
            get
            {
                _sb.Clear();
                foreach (var p in _parent.GetType().GetProperties())
                {
                    var ptype = p.PropertyType;
                    if (ptype == typeof(DataErrorInfo)) continue;
                    if (ptype.GetInterface(nameof(IDataErrorInfo)) == null) continue;

                    var pvalue = p.GetValue(_parent, null);
                    if (pvalue == null) continue;

                    var error = (ptype.GetProperty("Error")?.GetValue(pvalue, null) as string) ?? "";
                    if (error == "") continue;

                    if (0 < _sb.Length) _sb.Append("; ");
                    _sb.Append(p.Name);
                    _sb.Append(":( ");
                    _sb.Append(error);
                    _sb.Append(" )");
                }
                foreach (var kv in _named_errors)
                {
                    if (string.IsNullOrWhiteSpace(kv.Value)) continue;

                    if (0 < _sb.Length) _sb.Append("; ");
                    _sb.Append(kv.Key);
                    _sb.Append(":\"");
                    _sb.Append(kv.Value);
                    _sb.Append('"');
                }
                return (0 < _sb.Length) ? _sb.ToString() : "";
            }
        }

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _named_errors.TryGetValue(propertyName, out string? value) ? value : string.Empty;
            set
            {
                if ((_named_errors.TryGetValue(propertyName, out string? v) ? v : null) != value)
                {
                    _named_errors[propertyName] = value;
                    DataErrorInfoChanged?.Invoke(this, new DataErrorInfoChangedEventArgs(propertyName, value));
                }
            }
        }

        /// <summary>
        /// Property setter for value and error info
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <param name="forceUpdate">True: always set the new property value; False set only if not equal.</param>
        /// <param name="setter">optional: override setter action lvalue=value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        public bool TrySet<T>(
            ref T lvalue,
            T value,
            string? error = null,
            bool forceUpdate = false,
            Action<T?>? setter = null,
            [CallerMemberName] string propertyName = "")
            where T: IEquatable<T>
        {
            if (error != null) this[propertyName] = error;
            if (!forceUpdate && (lvalue?.Equals(value) ?? false)) return false;
            if (setter != null) setter(value);
            else lvalue = value;
            return true;
        }

        /// <summary>
        /// Property setter for value and error info
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <param name="forceUpdate">True: always set the new property value; False set only if not equal.</param>
        /// <param name="setter">optional: override setter action lvalue=value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        public bool TrySetRefNullable<T>(
            ref T? lvalue,
            T value,
            string? error = null,
            bool forceUpdate = false,
            Action<T?>? setter = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T>
        {
            if (error != null) this[propertyName] = error;
            if (!forceUpdate && (lvalue?.Equals(value) ?? false)) return false;
            if (setter != null) setter(value);
            else lvalue = value;
            return true;
        }

        /// <summary>
        /// Property setter for value and error info
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <param name="forceUpdate">True: always set the new property value; False set only if not equal.</param>
        /// <param name="setter">optional: override setter action lvalue=value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        public bool TrySetNullable<T>(
            ref T? lvalue,
            T? value,
            string? error = null,
            bool forceUpdate = false,
            Action<T?>? setter = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T?>
        {
            if (error != null) this[propertyName] = error;
            if (!forceUpdate)
            {
                if ((lvalue == null) && (value == null)) return false;
                if ((lvalue != null) && lvalue.Equals(value)) return false;
            }
            if (setter == null) lvalue = value;
            else setter(value);
            return true;
        }

        /// <summary>
        /// Property setter for enum value and error info
        /// </summary>
        /// <typeparam name="T">Property enum type</typeparam>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <param name="forceUpdate">True: always set the new property value; False set only if not equal.</param>
        /// <param name="setter">optional: override setter action lvalue=value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        public bool TrySetEnum<T>(
            ref T lvalue,
            T value,
            string? error = null,
            bool forceUpdate = false,
            Action<T?>? setter = null,
            [CallerMemberName] string propertyName = "")
            where T : Enum
        {
            if (error != null) this[propertyName] = error;
            if (!!forceUpdate && lvalue.Equals(value)) return false;
            if (setter == null) lvalue = value;
            else setter(value);
            return true;
        }

        /// <summary>
        /// Property setter for enum value and error info
        /// </summary>
        /// <typeparam name="T">Property enum type</typeparam>
        /// <param name="lvalue">Container for property value</param>
        /// <param name="value">New property value</param>
        /// <param name="error">Property error data for IDataErrorInfo, if null IDataErrorInfo is not updated.</param>
        /// <param name="forceUpdate">True: always set the new property value; False set only if not equal.</param>
        /// <param name="setter">optional: override setter action lvalue=value</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>True if value is assigned to lvalue; false otherwise</returns>
        public bool TrySetEnumNullable<T>(
            ref T? lvalue,
            T? value,
            string? error = null,
            bool forceUpdate = false,
            Action<T?>? setter = null,
            [CallerMemberName] string propertyName = "")
            where T : Enum
        {
            if (error != null) this[propertyName] = error;
            if (!forceUpdate)
            {
                if ((lvalue == null) && (value == null)) return false;
                if ((lvalue != null) && lvalue.Equals(value)) return false;
            }
            if (setter != null) setter(value);
            else lvalue = value;
            return true;
        }
    }

    public delegate void DataErrorInfoChangedEventHandler(object? sender, DataErrorInfoChangedEventArgs e);

    public class DataErrorInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the DataErrorInfoChangedEventArgs class.
        /// </summary>
        public DataErrorInfoChangedEventArgs(string? propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Indicates the name of the property that changed.
        /// </summary>
        public virtual string? PropertyName { get; }

        /// <summary>
        /// The error message with the property that changed.
        /// </summary>
        public string? ErrorMessage { get; }
    }
}
