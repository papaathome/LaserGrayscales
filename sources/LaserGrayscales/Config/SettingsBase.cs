using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

using As.Applications.Converters;
using As.Applications.Data;
using As.Applications.Data.Patterns;
using As.Applications.IO;
using As.Applications.Validation;

namespace As.Applications.Config
{
    [XmlInclude(typeof(GCodeSettings))]
    [XmlInclude(typeof(MachineSettings))]
    [XmlInclude(typeof(Settings))]
    [XmlInclude(typeof(TestPattern))]
    public abstract class SettingsBase : IDataErrorInfo, IChanged, IXmlProcessing
    {
        public SettingsBase()
        {
            _dataErrorInfo = new(this);
            Initialise();
            PropertyChanged += OnChanged;
        }

        abstract protected void Initialise();

        #region Properties
        [XmlAnyElement]
        public XmlComment Comment
        {
            get { return new XmlDocument().CreateComment(" Metric: all lengths in 'mm', all speeds in 'mm/sec' "); }
            set { }
        }

        /// <summary>
        /// Container for all values as string (expensive form of boxing!)
        /// </summary>
        [XmlIgnore]
        public ObservableCollection<KVPair> Content { get; private set; } = [];

        /// <summary>
        /// Container *only* used during XML serialisation. (Use Content and not this property.)
        /// </summary>
        [XmlArray("settings"), XmlArrayItem("setting")]
        public List<KVPair> XmlContent { get; set; } = [];
        #endregion Properties

        #region Actions
        protected bool TryGetValue(string name, out KVPair? result)
        {
            result = Content.FirstOrDefault(e => e.Key == name);
            return (result != null);
        }

        /// <summary>
        /// Set properties to some content, add new if not found.
        /// <remarks>This version preserves the original validator and comment</remarks>
        /// </summary>
        /// <param name="key">key of the content</param>
        /// <param name="value">new value of the content</param>
        /// <param name="comment">optional new comment of the content</param>
        /// <param name="validator">optional new validator of the content</param>
        protected void SetValue(
            string key,
            string? value,
            string? comment = null,
            KVPair.OnValidate? validator = null)
        {
            if (TryGetValue(key, out KVPair? r) && r != null)
            {
                r.Value = value;
                if (comment != null) r.Comment = comment;
                if (validator != null) r.TryValidate = validator;
            }
            else
            {
                var v =  new KVPair()
                {
                    Key = key,
                    Value = value,
                    Comment = comment ?? "",
                    TryValidate = validator
                };
                v.PropertyChanged += OnPropertyChanged;
                Content.Add(v);
            }
        }

        public void ImportValues(SettingsBase other)
        {
            foreach (var e in other.Content)
            {
                if (e == null) continue;
                SetValue(e.Key, e.Value, e.Comment, e.TryValidate);
            }
        }
        #endregion Actions

        #region IXmlProcessing
        /// <inheritdoc/>
        void IXmlProcessing.XmlWritePrepare() => WriteXmlPrepare();

        /// <summary>
        /// Prepare data for writing to XML.
        /// </summary>
        protected virtual void WriteXmlPrepare()
        {
            XmlContent = [.. Content];
        }

        /// <inheritdoc/>
        void IXmlProcessing.XmlWriteProcess() => WriteXmlProcess();

        /// <summary>
        /// Process data after writing to XML.
        /// </summary>
        protected virtual void WriteXmlProcess() { }

        /// <summary>
        /// Prepare data for reading from XML.
        /// </summary>
        protected virtual void ReadXmlPrepare() { }

        /// <inheritdoc/>
        void IXmlProcessing.XmlReadProcess() => ReadXmlProcess();

        /// <summary>
        /// Process data after reading from XML.
        /// </summary>
        protected virtual void ReadXmlProcess()
        {
            foreach (var v in XmlContent)
            {
                var kv = Content.FirstOrDefault(e => e.Key == v.Key);
                if (kv != null)
                {
                    kv.Value = v.Value;
                    kv.Comment = v.Comment;
                }
                else
                {
                    Content.Add(new KVPair()
                    {
                        Key = v.Key,
                        Value = v.Value,
                        Comment = v.Comment
                    });
                }
            }
            XmlContent.Clear();
        }
        #endregion IXmlProcessing

        #region IChanged
        [XmlIgnore]
        public bool IsChanged { get; protected set; } = true;

        void OnChanged(object? sender, PropertyChangedEventArgs e) => IsChanged = true;

        public void ResetChanged() => IsChanged = false;
        #endregion IChanged

        #region INotifyPropertyChanged
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnChanged(sender, e);
            PropertyChanged?.Invoke(this, e);
        }
        #endregion INotifyPropertyChanged

        #region IDataErrorInfo
        protected readonly DataErrorInfo _dataErrorInfo;

        /// <inheritdoc/>
        public string Error => _dataErrorInfo.Error;

        /// <inheritdoc/>
        public string this[string propertyName]
        {
            get => _dataErrorInfo[propertyName];
            internal set => _dataErrorInfo[propertyName] = value;
        }

        protected string? GetString(
            [CallerMemberName] string propertyName = "")
        {
            if (propertyName == "") return null;
            if (!TryGetValue(propertyName, out KVPair? result)) return null;
            if ((result == null) || (result.Value == null)) return null;
            return result.Value;
        }

        protected T? Get<T>(
            [CallerMemberName] string propertyName = "")
            where T : IParsable<T>
        {
            if (propertyName == "") return default;
            if (!TryGetValue(propertyName, out KVPair? result)) return default;
            if ((result == null) || (result.Value == null)) return default;
            return result.Value.CiNullable<T>();
        }
        #endregion IDataErrorInfo
    }
}
