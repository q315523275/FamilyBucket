using Bucket.SkrTrace.Core.Abstractions;
using Bucket.SkrTrace.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.SkrTrace.Transport.EventBus
{
    public static class SkrTraceBuilderExtensions
    {
        public static ISkrTraceBuilder AddEventBusTransport(this ISkrTraceBuilder extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }
            extensions.Services.AddSingleton<ISkrTraceCollect, SkrTraceEventBusTransport>();
            return extensions;
        }
    }
}
