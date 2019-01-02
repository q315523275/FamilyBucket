namespace Bucket.SkrTrace.Core.Abstractions
{
    public interface IRuntimeEnvironment
    {
        string ApplicationCode { get; set; }
        bool Initialized { get; }
    }
}
