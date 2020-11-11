using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Love2u.Profiles.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services) 
        {
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}
