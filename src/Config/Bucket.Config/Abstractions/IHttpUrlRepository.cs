namespace Bucket.Config.Abstractions
{
    public interface IHttpUrlRepository
    {
        string GetApiUrl(long version);
    }
}
