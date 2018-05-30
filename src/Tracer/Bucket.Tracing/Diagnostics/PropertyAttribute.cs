
namespace Bucket.Tracing.Diagnostics
{
    public class PropertyAttribute : ParameterBinder
    {
        public string Name { get; set; }

        public override object Resolve(object value)
        {
            if (value == null || Name == null)
            {
                return null;
            }

            var property = value.GetType().GetProperty(Name);
            
            return property?.GetValue(value);
        }
    }
}