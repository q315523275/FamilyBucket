using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.SkrTrace.Diagnostics.AspNetCore
{
    public static class SkrTraceBuilderExtensions
    {
        public static ISkrTraceBuilder AddAspNetCoreHosting(this ISkrTraceBuilder extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            extensions.Services.AddSingleton<ITracingDiagnosticProcessor, HostingDiagnosticProcessor>();
            return extensions;
        }
    }
}
