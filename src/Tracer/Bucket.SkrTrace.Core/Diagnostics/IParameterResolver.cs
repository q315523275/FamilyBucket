
namespace Bucket.SkrTrace.Core.Diagnostics
{
    public interface IParameterResolver
    {
        object Resolve(object value);
    }
}