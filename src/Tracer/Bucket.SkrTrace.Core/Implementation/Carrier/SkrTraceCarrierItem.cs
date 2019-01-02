using Bucket.SkrTrace.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.Implementation.Carrier
{
    public class SkrTraceCarrierItem : CarrierItem
    {
        private const string HEADER_NAME = "SkrTrace";
        private readonly IContextCarrier _carrier;

        public SkrTraceCarrierItem(IContextCarrier carrier, CarrierItem next, string @namespace)
            : base(HEADER_NAME, carrier.Serialize(), next, @namespace)
        {
            _carrier = carrier;
        }

        public override string HeadValue
        {
            get => base.HeadValue;
            set => _carrier.Deserialize(value);
        }
    }
}
