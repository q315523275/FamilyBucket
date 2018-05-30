
namespace Bucket.Tracing.Diagnostics
{
    public class NullParameterResolver : IParameterResolver
    {
        public object Resolve(object value)
        {
            return null;
        }
    }
}