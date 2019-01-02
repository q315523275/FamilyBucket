using System.Collections.Generic;

namespace Bucket.SkrTrace.Core.Utils
{
    public static class StackExtensions
    {
        public static bool TryPeek<T>(this Stack<T> stack, out T value)
        {
            if (stack == null || stack.Count == 0)
            {
                value = default(T);
                return false;
            }

            value = stack.Peek();
            return true;
        }

        public static bool TryPop<T>(this Stack<T> stack, out T value)
        {
            if (stack == null || stack.Count == 0)
            {
                value = default(T);
                return false;
            }

            value = stack.Pop();
            return true;
        }
    }
}
