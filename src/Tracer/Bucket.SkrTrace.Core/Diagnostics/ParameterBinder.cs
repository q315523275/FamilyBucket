
using System;

namespace Bucket.SkrTrace.Core.Diagnostics
{
    public abstract class ParameterBinder : Attribute, IParameterResolver
    {
        public abstract object Resolve(object value);
    }
}