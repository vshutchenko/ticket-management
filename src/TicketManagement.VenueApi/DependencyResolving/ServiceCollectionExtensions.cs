using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.DataAccess.Entities;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.VenueApi.DependencyResolving
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkRepositories(connectionString);

            services.AddScoped<IValidator<Area>, AreaValidator>();
            services.AddScoped<IValidator<Layout>, LayoutValidator>();
            services.AddScoped<IValidator<Seat>, SeatValidator>();
            services.AddScoped<IValidator<Venue>, VenueValidator>();

            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IVenueService, VenueService>();

            return services;
        }
    }
}
