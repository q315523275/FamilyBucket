namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface ITracerContext
    {
        void Inject(IContextCarrier carrier);

        void Extract(IContextCarrier carrier);

        ISpan ActiveSpan { get; }

        ISpan CreateEntrySpan(string operationName);

        ISpan CreateExitSpan(string operationName, string remotePeer);

        void StopSpan(ISpan span);

        void SetIdentity(string identity);
    }
}
