using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestEase;
using RestEase.Implementation;
using TicketManagement.VenueApi.Clients.UserApi;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal class TestingVenueApiFactory : WebApplicationFactory<VenueApi.Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
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

            return builder.Build();
        }
    }
}
