using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.PurchaseApi.Services.Implementations;
using TicketManagement.PurchaseApi.Services.Interfaces;

namespace TicketManagement.PurchaseApi.DependencyResolving
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkRepositories(connectionString);

            services.AddScoped<IPurchaseService, PurchaseService>();

            return services;
        }
    }
}
