using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.DataAccess.Data;
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

            services.AddDefaultIdentity<User>().AddRoles<IdentityRole>().AddEntityFrameworkStores<TicketManagementContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
            });

            services.AddScoped<IRepository<Area>, AreaRepository>();
            services.AddScoped<IRepository<EventArea>, EventAreaRepository>();
            services.AddScoped<IRepository<EventSeat>, EventSeatRepository>();
            services.AddScoped<IRepository<Event>, EventRepository>();
            services.AddScoped<IRepository<Layout>, LayoutRepository>();
            services.AddScoped<IRepository<Seat>, SeatRepository>();
            services.AddScoped<IRepository<Venue>, VenueRepository>();
            services.AddScoped<IRepository<Purchase>, PurchaseRepository>();
            services.AddScoped<IRepository<PurchasedSeat>, PurchasedSeatRepository>();

            return services;
        }
    }
}
