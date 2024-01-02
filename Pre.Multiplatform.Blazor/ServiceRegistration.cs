using Microsoft.AspNetCore.Components;
using Pre.MultiPlatform.Blazor.Integration;
namespace Pre.Multiplatform.Blazor
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var apiUri = "http://34.41.141.60:8080/";
            services.AddSingleton<HttpClient>();
            services.AddTransient<VideoStreamingApiHandler>(provider => new VideoStreamingApiHandler(apiUri, provider.GetRequiredService<HttpClient>()));

            return services;
        }
    }
}
