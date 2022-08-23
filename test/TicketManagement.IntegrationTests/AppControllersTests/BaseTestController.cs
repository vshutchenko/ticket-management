using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RestEase;
using TicketManagement.IntegrationTests.Addition;
using TicketManagement.IntegrationTests.Factories;
using TicketManagement.WebApplication.Clients.EventApi;
using TicketManagement.WebApplication.Clients.PurchaseApi;
using TicketManagement.WebApplication.Clients.UserApi;
using TicketManagement.WebApplication.Clients.VenueApi;

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
                        d => d.ServiceType == typeof(EventApi.Clients.UserApi.IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<EventApi.Clients.UserApi.IUserClient>(userApiCLient);

                    services.AddScoped(p => userClient);
                }));

            VenueApiFactory = new WebApplicationFactory<VenueApi.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.ReplaceContextWithTestDb();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(VenueApi.Clients.UserApi.IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<VenueApi.Clients.UserApi.IUserClient>(userApiCLient);

                    services.AddScoped(p => userClient);
                }));

            PurchaseApiFactory = new WebApplicationFactory<PurchaseApi.Program>()
                .WithWebHostBuilder(c => c.ConfigureServices(services =>
                {
                    services.ReplaceContextWithTestDb();

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(PurchaseApi.Clients.UserApi.IUserClient));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var userClient = RestClient.For<PurchaseApi.Clients.UserApi.IUserClient>(userApiCLient);

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

                    var types = new List<Type>
                    {
                        typeof(IEventClient),
                        typeof(IEventAreaClient),
                        typeof(IEventSeatClient),
                        typeof(IVenueClient),
                        typeof(ILayoutClient),
                        typeof(IUserClient),
                        typeof(IPurchaseClient),
                    };

                    foreach (var t in types)
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
                    var userClient = RestClient.For<IUserClient>(userApiCLient);
                    var purchaseClient = RestClient.For<IPurchaseClient>(purchaseApiCLient);

                    services.AddScoped(p => eventClient);
                    services.AddScoped(p => eventAreaClient);
                    services.AddScoped(p => eventSeatClient);
                    services.AddScoped(p => layoutClient);
                    services.AddScoped(p => venueClient);
                    services.AddScoped(p => userClient);
                    services.AddScoped(p => purchaseClient);
                }));
        }
    }
}
