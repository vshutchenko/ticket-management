using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestEase;
using TicketManagement.IntegrationTests.Addition;

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

                services.AddScoped<ContextSeeder>();

                var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(Core.Clients.UserApi.IUserClient));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var userApiCLient = new WebApplicationFactory<UserApi.Program>().CreateClient();

                var userClient = RestClient.For<Core.Clients.UserApi.IUserClient>(userApiCLient);

                services.AddScoped(p => userClient);

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var seeder = scope.ServiceProvider.GetRequiredService<ContextSeeder>();

                seeder.SeedInitialDataAsync().Wait();
            });
        }
    }
}
