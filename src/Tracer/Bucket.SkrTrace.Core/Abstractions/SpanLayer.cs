namespace Bucket.SkrTrace.Core.Abstractions
{
    public enum SpanLayer
    {
        DB = 1,
        RPC_FRAMEWORK = 2,
        HTTP = 3,
        MQ = 4,
        CACHE = 5
    }
}
