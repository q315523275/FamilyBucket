
using System;

namespace Bucket.Tracing.Diagnostics
{
    public abstract class ParameterBinder : Attribute, IParameterResolver
    {
        public abstract object Resolve(object value);
    }
}