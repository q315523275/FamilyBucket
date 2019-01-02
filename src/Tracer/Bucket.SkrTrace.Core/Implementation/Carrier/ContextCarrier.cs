using Bucket.SkrTrace.Core.Abstractions;
using Newtonsoft.Json;
namespace Bucket.SkrTrace.Core.Implementation.Carrier
{
    public class ContextCarrier : IContextCarrier
    {
        private string _identity = string.Empty;
        public string TraceSegmentId { set; get; }
        public string SpanId { set; get; }
        public string Identity { set { _identity = value; } get { return _identity; } }
        public string EntryApplicationCode { set; get; }
        public string PeerHost { set; get; }
        public string EntryOperationName { set; get; }
        public string ParentApplicationCode { set; get; }
        public string ParentOperationName { set; get; }
        [JsonIgnore]
        public bool IsValid {
            get {
                return !string.IsNullOrWhiteSpace(TraceSegmentId)
                       && !string.IsNullOrWhiteSpace(SpanId)
                       && !string.IsNullOrWhiteSpace(EntryApplicationCode)
                       && !string.IsNullOrWhiteSpace(EntryOperationName)
                       && !string.IsNullOrWhiteSpace(ParentOperationName)
                       && !string.IsNullOrWhiteSpace(ParentOperationName)
                       && !string.IsNullOrWhiteSpace(PeerHost);
            }
        }
        [JsonIgnore]
        public CarrierItem Items
        {
            get
            {
                var carrierItem = new SkrTraceCarrierItem(this, null, string.Empty);
                var head = new CarrierItemHead(carrierItem, string.Empty);
                return head;
            }
        }
        public IContextCarrier Deserialize(string text)
        {
            if(!string.IsNullOrWhiteSpace(text) && text.StartsWith("{") && text.EndsWith("}"))
            {
                var item = JsonConvert.DeserializeObject<ContextCarrier>(text);
                this.TraceSegmentId = item?.TraceSegmentId;
                this.SpanId = item?.SpanId;
                this.Identity = item?.Identity;
                this.EntryApplicationCode = item?.EntryApplicationCode;
                this.EntryOperationName = item?.EntryOperationName;
                this.ParentApplicationCode = item?.ParentApplicationCode;
                this.ParentOperationName = item?.ParentOperationName;
                this.PeerHost = item?.PeerHost;
            }
            return this;
        }

        public string Serialize()
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
