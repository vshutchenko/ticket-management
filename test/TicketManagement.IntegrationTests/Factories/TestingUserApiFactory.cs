using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using TicketManagement.Core.Clients.UserApi;
using TicketManagement.IntegrationTests.Addition;
using TicketManagement.UserApi.Data;

namespace TicketManagement.IntegrationTests.Factories
{
    internal class TestingUserApiFactory : WebApplicationFactory<UserApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.ReplaceContextWithTestDb();

                new TestDatabase.TestDatabaseInfo().CreateDb();

                var servicesToRemove = new List<Type>
                        {
                            typeof(UserApi.Data.ContextSeeder),
                            typeof(IUserClient),
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

                services.AddScoped<IContextSeeder, Addition.ContextSeeder>();

                var userApiCLient = new WebApplicationFactory<UserApi.Program>().WithWebHostBuilder(b =>
                {
                    b.ConfigureServices(s =>
                    {
                        s.ReplaceContextWithTestDb();

                        new TestDatabase.TestDatabaseInfo().CreateDb();

                        var servicesToRemove = new List<Type>
                        {
                            typeof(UserApi.Data.ContextSeeder),
                            typeof(IUserClient),
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

                        s.AddScoped<IContextSeeder, Addition.ContextSeeder>();
                    });
                }).CreateClient();

                var userClient = RestClient.For<IUserClient>(userApiCLient);

                services.AddScoped(p => userClient);

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var seeder = scope.ServiceProvider.GetRequiredService<IContextSeeder>();

                seeder.SeedInitialDataAsync().Wait();
            });
        }
    }
}
