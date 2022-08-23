using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var seeder = scope.ServiceProvider.GetRequiredService<ContextSeeder>();

                seeder.SeedInitialDataAsync().Wait();
            });
        }
    }
}
