using Microsoft.Extensions.DependencyInjection;

namespace Bucket.DependencyInjection
{
    /// <summary>
    /// Cap options extension
    /// </summary>
    public interface IOptionsExtension
    {
        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection"/></param>
        void AddServices(IServiceCollection services);
    }
}
