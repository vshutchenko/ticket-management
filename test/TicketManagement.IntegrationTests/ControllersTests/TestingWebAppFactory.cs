using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.IntegrationTests.ControllersTests.Addition;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal class TestingWebAppFactory : WebApplicationFactory<WebApplication.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddAntiforgery(t =>
                {
                    t.Cookie.Name = AntiForgeryTokenExtractor.Cookie;
                    t.FormFieldName = AntiForgeryTokenExtractor.Field;
                });
            });
        }
    }
}
