using System.Collections;
using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.Abstractions
{
    public class CarrierItem : IEnumerable<CarrierItem>
    {
        private readonly string _headKey;
        private string _headValue;
        private readonly CarrierItem _next;

        public virtual string HeadKey => _headKey;

        public virtual string HeadValue
        {
            get => _headValue;
            set => _headValue = value;
        }

        protected CarrierItem(string headKey, string headValue, string @namespace)
            : this(headKey, headValue, null, @namespace)
        {
        }

        protected CarrierItem(string headKey, string headValue, CarrierItem next, string @namespace)
        {
            _headKey = string.IsNullOrEmpty(@namespace) ? headKey : $"{@namespace}-{headKey}";
            _headValue = headValue;
            _next = next;
        }

        public IEnumerator<CarrierItem> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private class Enumerator : IEnumerator<CarrierItem>
        {
            private readonly CarrierItem _head;

            public CarrierItem Current { get; private set; }

            object IEnumerator.Current => Current;

            public Enumerator(CarrierItem head)
            {
                _head = head;
                Current = head;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var next = Current._next;
                if (next == null)
                {
                    return false;
                }

                Current = next;
                return true;
            }

            public void Reset()
            {
                Current = _head;
            }
        }
    }
}
