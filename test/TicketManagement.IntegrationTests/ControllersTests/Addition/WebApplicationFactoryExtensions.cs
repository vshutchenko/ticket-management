using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace TicketManagement.IntegrationTests.ControllersTests.Addition
{
    internal static class TestingWebAppFactoryExtensions
    {
        public static WebApplicationFactory<TicketManagement.WebApplication.Program> WithAuthentication(
            this WebApplicationFactory<TicketManagement.WebApplication.Program> factory,
            TestClaimsProvider claimsProvider)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => claimsProvider);
                });
            });
        }

        public static HttpClient CreateClientWithTestAuth(this WebApplicationFactory<TicketManagement.WebApplication.Program> factory, TestClaimsProvider claimsProvider)
        {
            var client = factory.WithAuthentication(claimsProvider).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", new TokenHelper().GetToken(claimsProvider.Claims));

            return client;
        }
    }
}
