using System.Collections;
using System.Xml.Serialization;

namespace As.Applications.Models
{
    public class Scale : IEquatable<Scale>
    {
        public Scale() { }

        internal Scale(Scale other)
        {
            First = other.First;
            Last = other.Last;
            Step = other.Step;
            Increment = other.Increment;
        }

        #region IEquatable
        public bool Equals(Scale? other)
        {
            if (other == null) return false;
            return
                (First == other.First) &&
                (Last == other.Last) &&
                (Step == other.Step) &&
                (Increment == other.Increment);
        }

        public override bool Equals(object? obj)
        {
            if ((obj == null) || (obj is not Scale)) return false;
            return Equals(obj as Scale);
        }

        public override int GetHashCode()
        {
            int result = 0x5aa55aa5;
            result ^= First.GetHashCode();
            result ^= Last.GetHashCode();
            result ^= Step.GetHashCode();
            result ^= Increment.GetHashCode();
            return result;
        }
        #endregion IEquatable

        #region Data
        [XmlAttribute(AttributeName = "first")]
        public int First { get; set; } = 0;

        [XmlAttribute(AttributeName = "last")]
        public int Last { get; set; } = 100;

        [XmlAttribute(AttributeName = "step")]
        public int Step { get; set; } = 1;

        [XmlAttribute(AttributeName = "increment")]
        public double Increment { get; set; } = 1;
        #endregion Data

        [XmlIgnore]
        public double Range
            => (Math.Abs(Last - First) + 1) * Math.Abs(Increment) / Step;

        public double GetGroupedRange(int group_size, double group_gap)
            => (Math.Abs(Last - First) + 1) * (Math.Abs(Increment) / Step + group_gap / group_size);

        public IEnumerator<double> GetEnumerator()
            => new ScaleEnumerator(this);
    }

    public class ScaleEnumerator : IEnumerator<double>, IEnumerator
    {
        public ScaleEnumerator(Scale scale)
        {
            _scale = scale;
            Reset();
        }

        private readonly Scale _scale;
        private double _current;

        private double _step;
        private bool _reversed;

        public double Current => _current;

        object IEnumerator.Current => Current;

        public void Reset()
        {
            _reversed = (_scale.Last < _scale.First);

            _step = _scale.Step;
            if (
                !_reversed && (_step < 0) ||
                _reversed && (0 < _step)
               )
            {
                _step = -_step;
            }

            _current = _scale.First - _step;
        }

        public bool MoveNext()
        {
            _current += _step;
            return
                _reversed && (_scale.Last <= _current) ||
                !_reversed && (_current <= _scale.Last);
        }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        {
            // not using resources, nothing to do;
        }
    }
}
