using System.Threading.Tasks;

namespace Tracing.Common
{
    public static class TaskUtils
    {
        public static readonly Task<bool> FailCompletedTask = Task.FromResult(false);
        
        public static readonly Task<bool> CompletedTask = Task.FromResult(true);
    }

    public static class TaskUtils<T>
    {
        public static readonly Task<T> FromDefault = Task.FromResult(default(T));
    }
}