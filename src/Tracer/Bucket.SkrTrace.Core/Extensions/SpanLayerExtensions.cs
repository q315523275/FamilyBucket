using Bucket.SkrTrace.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bucket.SkrTrace.Core.Extensions
{
    public static class SpanLayerExtensions
    {
        public static void AsDB(this ISpan span)
        {
            span.SetLayer(SpanLayer.DB);
        }

        public static void AsCache(this ISpan span)
        {
            span.SetLayer(SpanLayer.CACHE);
        }

        public static void AsRPCFramework(this ISpan span)
        {
            span.SetLayer(SpanLayer.RPC_FRAMEWORK);
        }

        public static void AsHttp(this ISpan span)
        {
            span.SetLayer(SpanLayer.HTTP);
        }

        public static void AsMQ(this ISpan span)
        {
            span.SetLayer(SpanLayer.MQ);
        }
    }
}
