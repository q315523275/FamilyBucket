
using Bucket.SkrTrace.Core.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bucket.SkrTrace.Core.Diagnostics
{
    internal class TracingDiagnosticMethod
    {
        private readonly MethodInfo _method;
        private readonly ITracingDiagnosticProcessor _tracingDiagnosticProcessor;
        private readonly string _diagnosticName;
        private readonly IParameterResolver[] _parameterResolvers;

        public TracingDiagnosticMethod(ITracingDiagnosticProcessor tracingDiagnosticProcessor, MethodInfo method,
            string diagnosticName)
        {
            _tracingDiagnosticProcessor = tracingDiagnosticProcessor;
            _method = method;
            _diagnosticName = diagnosticName;
            _parameterResolvers = GetParameterResolvers(method).ToArray();
        }

        public void Invoke(string diagnosticName, object value)
        {
            if (_diagnosticName != diagnosticName)
            {
                return;
            }

            var args = new object[_parameterResolvers.Length];
            for (var i = 0; i < _parameterResolvers.Length; i++)
            {
                args[i] = _parameterResolvers[i].Resolve(value);
            }

            _method.Invoke(_tracingDiagnosticProcessor, args);
        }

        private static IEnumerable<IParameterResolver> GetParameterResolvers(MethodInfo methodInfo)
        {
            foreach (var parameter in methodInfo.GetParameters())
            {
                var binder = parameter.GetCustomAttribute<ParameterBinder>();
                if (binder != null)
                {
                    if(binder is ObjectAttribute objectBinder)
                    {
                        if (objectBinder.TargetType == null)
                        {
                            objectBinder.TargetType = parameter.ParameterType;
                        }
                    }
                    if(binder is PropertyAttribute propertyBinder)
                    {
                        if (propertyBinder.Name == null)
                        {
                            propertyBinder.Name = parameter.Name;
                        }
                    }
                    yield return binder;
                }
                else
                {
                    yield return new NullParameterResolver();
                }
            }
        }
    }
}