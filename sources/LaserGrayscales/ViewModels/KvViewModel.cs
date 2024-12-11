using System.Collections.ObjectModel;
using System.ComponentModel;

using As.Applications.Data;
using As.Applications.Validation;

using Caliburn.Micro;

namespace As.Applications.ViewModels
{
    internal class KvViewModel : Screen, IDataErrorInfo
    {
        public KvViewModel()
        {
            _dataErrorInfo = new(this);
        }

        public required ObservableCollection<KVPair> KV
        {
            get => _kv;
            set
            {
                _kv = value;
                NotifyOfPropertyChange();
            }
        }
        ObservableCollection<KVPair> _kv = new ObservableCollection<KVPair>();

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
        #endregion IDataErrorInfo
    }
}
