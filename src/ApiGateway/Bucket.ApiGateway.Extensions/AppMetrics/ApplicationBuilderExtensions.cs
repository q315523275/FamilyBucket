using Microsoft.AspNetCore.Builder;
namespace Bucket.ApiGateway.Extensions.AppMetrics
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAppMetrics(this IApplicationBuilder app)
        {
            // To add all available tracking middleware
            app.UseMetricsAllMiddleware();

            // Or to cherry-pick the tracking of interest
            // app.UseMetricsActiveRequestMiddleware();
            // app.UseMetricsErrorTrackingMiddleware();
            // app.UseMetricsPostAndPutSizeTrackingMiddleware();
            // app.UseMetricsRequestTrackingMiddleware();
            // app.UseMetricsOAuth2TrackingMiddleware();
            // app.UseMetricsApdexTrackingMiddleware();

            // To add all supported endpoints
            app.UseMetricsAllEndpoints();

            // Or to cherry-pick endpoint of interest
            // app.UseMetricsEndpoint();
            // app.UseMetricsTextEndpoint();
            // app.UseEnvInfoEndpoint();

            return app;
        }
    }
}
