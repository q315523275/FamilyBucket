using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.SkrTrace.Diagnostics.HttpClient
{
    public static class SkrTraceBuilderExtensions
    {
        public static ISkrTraceBuilder AddHttpClient(this ISkrTraceBuilder extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            extensions.Services.AddSingleton<ITracingDiagnosticProcessor, HttpClientDiagnosticProcessor>();
            return extensions;
        }
    }
}
