using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RestEase;
using RestEase.Implementation;
using TicketManagement.Core.Clients.UserApi;

namespace TicketManagement.IntegrationTests.Factories
{
    internal class TestingEventApiFactory : WebApplicationFactory<EventApi.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IUserClient));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var userApiRequester = new Requester(CreateClient());

                var userClient = RestClient.For<IUserClient>(userApiRequester);

                services.AddScoped(p => userClient);
            });
        }
    }
}
