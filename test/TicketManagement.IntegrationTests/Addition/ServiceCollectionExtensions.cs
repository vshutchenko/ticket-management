using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.DataAccess.EntityFrameworkImplementations;

namespace TicketManagement.IntegrationTests.Addition
{
    internal static class ServiceCollectionExtensions
    {
        public static void ReplaceContextWithTestDb(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TicketManagementContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var testDb = new TestDatabase.TestDatabaseInfo();

            services.AddDbContext<TicketManagementContext>(options =>
            {
                options.UseSqlServer(testDb.ConnectionString);
            });
        }
    }
}
