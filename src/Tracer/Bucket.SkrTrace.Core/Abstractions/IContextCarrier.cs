namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface IContextCarrier
    {
        string TraceSegmentId { get; set; }
        string SpanId { get; set; }
        string Identity { get; set; }
        string EntryApplicationCode { get; set; }
        string EntryOperationName { get; set; }
        string ParentApplicationCode { get; set; }
        string ParentOperationName { get; set; }
        string PeerHost { get; set; }
        bool IsValid { get; }
        IContextCarrier Deserialize(string text);
        string Serialize();
        CarrierItem Items { get; }
    }
}
