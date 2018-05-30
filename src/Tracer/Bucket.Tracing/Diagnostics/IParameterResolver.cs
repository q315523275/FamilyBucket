
namespace Bucket.Tracing.Diagnostics
{
    public interface IParameterResolver
    {
        object Resolve(object value);
    }
}