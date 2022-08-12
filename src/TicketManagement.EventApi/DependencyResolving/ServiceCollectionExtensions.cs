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

            services.AddScoped<IValidator<Area>, AreaValidator>();
            services.AddScoped<IValidator<decimal>, PriceValidator>();
            services.AddScoped<IValidator<Event>, EventValidator>();
            services.AddScoped<IValidator<Layout>, LayoutValidator>();
            services.AddScoped<IValidator<Seat>, SeatValidator>();
            services.AddScoped<IValidator<Venue>, VenueValidator>();

            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IEventAreaService, EventAreaService>();
            services.AddScoped<IEventSeatService, EventSeatService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IVenueService, VenueService>();

            return services;
        }
    }
}
