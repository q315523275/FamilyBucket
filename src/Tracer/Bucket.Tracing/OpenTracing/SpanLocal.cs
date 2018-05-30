using System.Collections.Generic;
using System.Threading;

namespace Bucket.OpenTracing
{
    internal static class SpanLocal
    {
        private static readonly AsyncLocal<Dictionary<string,ISpan>> AsyncLocal = new AsyncLocal<Dictionary<string, ISpan>>();
        public static Dictionary<string, ISpan> Current
        {
            get
            {
                if (AsyncLocal.Value == null)
                    AsyncLocal.Value = new Dictionary<string, ISpan>();
                return AsyncLocal.Value;
            }
            set
            {
                AsyncLocal.Value = value;
            }
        }
    }
}