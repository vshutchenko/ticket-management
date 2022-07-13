﻿using Microsoft.Extensions.DependencyInjection;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.DependencyResolving
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

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IPurchaseService, PurchaseService>();

            return services;
        }
    }
}