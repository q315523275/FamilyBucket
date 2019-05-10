using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bucket.SkyApm.Transport.EventBus
{
    public static class SkyWalkingBuilderExtensions
    {
        public static SkyApmExtensions UseEventBusTransport(this SkyApmExtensions extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            extensions.Services.AddSingleton<ISegmentReporter, EventBusSegmentReporter>();

            return extensions;
        }
    }
}
