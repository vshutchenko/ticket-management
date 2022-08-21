using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.IntegrationTests.ControllersTests.Addition;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal class TestingUserApiFactory : WebApplicationFactory<UserApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TicketManagementContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var testDb = new TestDatabase.TestDatabase();

                services.AddDbContext<TicketManagementContext>(options =>
                {
                    options.UseSqlServer(testDb.ConnectionString);
                });

                services.AddScoped<ContextSeeder>();

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var seeder = scope.ServiceProvider.GetRequiredService<ContextSeeder>();

                seeder.SeedInitialDataAsync().Wait();
            });
        }
    }
}
