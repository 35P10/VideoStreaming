using Core.App.Services;
using Integration.Repository;

namespace Pre.MultiPlatform
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ICloudStorageService, GoogleCloudStorageService>();
            return services;
        }
    }
}
