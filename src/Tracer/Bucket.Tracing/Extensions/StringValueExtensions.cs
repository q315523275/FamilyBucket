using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bucket.Tracing.Extensions
{
    internal static class StringValueExtensions
    {
        public static string GetValue(this StringValues stringValues)
        {
            if (stringValues.Count == 1)
            {
                return stringValues;
            }
            return stringValues.ToArray().LastOrDefault();
        }
    }
}
