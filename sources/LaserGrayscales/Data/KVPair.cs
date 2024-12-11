using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

using As.Applications.Converters;
using As.Applications.Validation;

namespace As.Applications.Data
{
    public class KVPair : IFormattable, IDataErrorInfo, INotifyPropertyChanged
    {
        // Problem serialising: KVP<string, string>, parameterless .ctor not accepted caused by the type arguments.
        // workaround: shield KVP<string, string> from XML serialiser

        public KVPair()
        {
            _kvpair = new KVP<string, string>()
            {
                Key = "",
                Value = null,
                Comment = "",
                TryValidate = ValidateHandler
            };
        }

        #region Properties
        readonly KVP<string, string> _kvpair;

        // Enable to check KVP streaming problem.
        //[XmlElement("kvp")]
        //public KVP<string, string> Kvp
        //{
        //    get => _kvpair;
        //    set
        //    {
        //        _kvpair.Key = value.Key;
        //        _kvpair.Value = value.Value;
        //        _kvpair.Comment = value.Comment;
        //    }
        //}

        [XmlElement("key")]
        public string Key
        {
            get => _kvpair.Key;
            set => _kvpair.Key = value;
        }

        [XmlElement("value")]
        public string? Value
        {
            get => _kvpair.Value;
            set => _kvpair.Value = value;
        }

        [XmlElement("comment")]
        public string Comment
        {
            get => _kvpair.Comment;
            set => _kvpair.Comment = value;
        }
        #endregion Properties

        #region Actions
        public delegate string OnValidate(string? value, string name = "");

        /// <summary>
        /// Optional validator action on Value.
        /// </summary>
        [XmlIgnore]
        public OnValidate? TryValidate
        {
            get => _tryValidate;
            set => _tryValidate = value;
        }
        OnValidate? _tryValidate = null;

        string ValidateHandler(string? value, string name = "") => TryValidate?.Invoke(value, name) ?? "";
        #endregion Actions

        #region IFormattable
        public override string ToString() => _kvpair.ToString(null, null);

        /// <summary>
        /// Readable form of a KVPair.
        /// </summary>
        /// <param name="format">Format to use on Value if TValue:IFormattable, default "G"</param>
        /// <param name="formatProvider">Format provider to use on Value if TValue:IFormattable, defaul CultureInvariant</param>
        /// <returns>Readable form of a KVPair</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => _kvpair.ToString(format, formatProvider);
        #endregion IFormattable

        #region INotifyPropertyChanged
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add => _kvpair.PropertyChanged += value;
            remove => _kvpair.PropertyChanged -= value;
        }
        #endregion INotifyPropertyChanged

        #region IDataErrorInfo
        /// <inheritdoc/>
        public string Error => _kvpair.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _kvpair[propertyName];
            internal set => _kvpair[propertyName] = value;
        }
        #endregion IDataErrorInfo
    }

    public class KVP<TKey, TValue> : IFormattable, IDataErrorInfo
        where TKey : notnull, IEquatable<TKey>
        where TValue : IEquatable<TValue?>
    {
        public KVP()
        {
            _dataErrorInfo = new(this);
        }

        #region Properties
        /// <summary>
        /// Key part of the pair, must have a value and preferrebly is set once.
        /// </summary>
        [XmlElement("key")]
        public TKey Key
        {
            get => _key;
            set => Set(ref _key, value);
        }
        TKey _key = default!;

        /// <summary>
        /// Value part of the pair, may be null, may change many times.
        /// </summary>
        [XmlElement("value")]
        public TValue? Value
        {
            get => _value;
            set => SetNullable(ref _value, value, TryValidate?.Invoke(value, $"{Key}"));
        }
        TValue? _value = default;

        [XmlElement("comment")]
        public string Comment { get; set; } = "";
        #endregion Properties

        #region Actions
        /// <summary>
        /// Optional validator action on Value.
        /// </summary>
        public delegate string OnValidate(TValue? value, string name = "");

        public OnValidate? TryValidate { get; set; } = null;
        #endregion Actions

        #region IFormattable
        public override string ToString() => ToString(null, null);

        /// <summary>
        /// Readable form of a KVPair.
        /// </summary>
        /// <param name="format">Format to use on Value if TValue:IFormattable, default "G"</param>
        /// <param name="formatProvider">Format provider to use on Value if TValue:IFormattable, defaul CultureInvariant</param>
        /// <returns>Readable form of a KVPair</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => (Value is string)
            ? $"{Key}=\"{(Value?.ToString() ?? "")}\""
            : $"{Key}={Value?.CiNullable(format, formatProvider) ?? ""}";
        #endregion IFormattable

        #region INotifyPropertyChanged
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyOfPropertyChange(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion INotifyPropertyChanged

        #region IDataErrorInfo
        readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        internal bool Set<T>(
            ref T lvalue,
            T value,
            string? error = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T>
        {
            if (!_dataErrorInfo.TrySet(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }

        internal bool SetNullable<T>(
            ref T? lvalue,
            T? value,
            string? error = null,
            [CallerMemberName] string propertyName = "")
            where T : IEquatable<T?>
        {
            if (!_dataErrorInfo.TrySetNullable(ref lvalue, value, error, propertyName: propertyName)) return false;
            NotifyOfPropertyChange(propertyName);
            return true;
        }
        #endregion IDataErrorInfo
    }
}
