using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.DependencyResolving
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TicketManagementContext>(options => options.UseSqlServer(connectionString));
            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<TicketManagementContext>();

            services.AddScoped<IRepository<Area>, AreaRepository>();
            services.AddScoped<IRepository<EventArea>, EventAreaRepository>();
            services.AddScoped<IRepository<EventSeat>, EventSeatRepository>();
            services.AddScoped<IRepository<Event>, EventRepository>();
            services.AddScoped<IRepository<Layout>, LayoutRepository>();
            services.AddScoped<IRepository<Seat>, SeatRepository>();
            services.AddScoped<IRepository<Venue>, VenueRepository>();

            return services;
        }
    }
}
