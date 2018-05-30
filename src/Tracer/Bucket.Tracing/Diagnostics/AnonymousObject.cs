namespace Bucket.Tracing.Diagnostics
{
    public class AnonymousObject : ParameterBinder
    {
        public override object Resolve(object value)
        {
            return value;
        }
    }
}