using TicketManagement.DataAccess.DependencyResolving;

namespace TicketManagement.PurchaseApi.DependencyResolving
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkRepositories(connectionString);

            return services;
        }
    }
}
