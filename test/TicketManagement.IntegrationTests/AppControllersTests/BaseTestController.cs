using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RestEase;
using TicketManagement.Core.Clients.EventApi;
using TicketManagement.Core.Clients.PurchaseApi;
using TicketManagement.Core.Clients.UserApi;
using TicketManagement.Core.Clients.VenueApi;
using TicketManagement.IntegrationTests.Addition;
using TicketManagement.IntegrationTests.Factories;

namespace TicketManagement.IntegrationTests.AppControllersTests
{
    internal abstract class BaseTestController
    {
        protected TestingUserApiFactory UserApiFactory { get; private set; }

        protected WebApplicationFactory<VenueApi.Program> VenueApiFactory { get; private set; }

        protected WebApplicationFactory<EventApi.Program> EventApiFactory { get; private set; }

        protected WebApplicationFactory<PurchaseApi.Program> PurchaseApiFactory { get; private set; }

        protected WebApplicationFactory<WebApplication.Program> AppFactory { get; private set; }

        [SetUp]
        public void SetUp()
        {
            UserApiFactory = new TestingUserApiFactory();
            var userApiCLient = UserApiFactory.CreateClient();

            EventApiFactory = new WebApplicationFactory<EventApi.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.ReplaceContextWithTestDb();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<IUserClient>(userApiCLient);

                    services.AddScoped(p => userClient);
                }));

            VenueApiFactory = new WebApplicationFactory<VenueApi.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.ReplaceContextWithTestDb();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<IUserClient>(userApiCLient);

                    services.AddScoped(p => userClient);
                }));

            PurchaseApiFactory = new WebApplicationFactory<PurchaseApi.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.ReplaceContextWithTestDb();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<IUserClient>(userApiCLient);

                    services.AddScoped(p => userClient);
                }));

            var eventApiCLient = EventApiFactory.CreateClient();
            var venueApiCLient = VenueApiFactory.CreateClient();
            var purchaseApiCLient = PurchaseApiFactory.CreateClient();

            AppFactory = new WebApplicationFactory<WebApplication.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.AddAntiforgery(t =>
                    {
                        t.Cookie.Name = AntiForgeryTokenExtractor.Cookie;
                        t.FormFieldName = AntiForgeryTokenExtractor.Field;
                    });

                    var servicesToRemove = new List<Type>
                    {
                        typeof(IEventClient),
                        typeof(IEventAreaClient),
                        typeof(IEventSeatClient),
                        typeof(IVenueClient),
                        typeof(ILayoutClient),
                        typeof(IAreaClient),
                        typeof(ISeatClient),
                        typeof(IUserClient),
                        typeof(IPurchaseClient),
                    };

                    foreach (var t in servicesToRemove)
                    {
                        var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == t);

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }
                    }

                    var eventClient = RestClient.For<IEventClient>(eventApiCLient);
                    var eventAreaClient = RestClient.For<IEventAreaClient>(eventApiCLient);
                    var eventSeatClient = RestClient.For<IEventSeatClient>(eventApiCLient);
                    var venueClient = RestClient.For<IVenueClient>(venueApiCLient);
                    var layoutClient = RestClient.For<ILayoutClient>(venueApiCLient);
                    var areaClient = RestClient.For<IAreaClient>(venueApiCLient);
                    var seatClient = RestClient.For<ISeatClient>(venueApiCLient);
                    var userClient = RestClient.For<IUserClient>(userApiCLient);
                    var purchaseClient = RestClient.For<IPurchaseClient>(purchaseApiCLient);

                    services.AddScoped(p => eventClient);
                    services.AddScoped(p => eventAreaClient);
                    services.AddScoped(p => eventSeatClient);
                    services.AddScoped(p => layoutClient);
                    services.AddScoped(p => venueClient);
                    services.AddScoped(p => areaClient);
                    services.AddScoped(p => seatClient);
                    services.AddScoped(p => userClient);
                    services.AddScoped(p => purchaseClient);
                }));
        }
    }
}
