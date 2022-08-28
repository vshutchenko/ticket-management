using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.EventApi.DependencyResolving
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkRepositories(connectionString);

            services.AddScoped<IValidator<decimal>, PriceValidator>();
            services.AddScoped<IValidator<Event>, EventValidator>();

            services.AddScoped<IEventAreaService, EventAreaService>();
            services.AddScoped<IEventSeatService, EventSeatService>();
            services.AddScoped<IEventService, EventService>();

            return services;
        }
    }
}
