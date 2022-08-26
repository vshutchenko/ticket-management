using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.Addition;

namespace TicketManagement.IntegrationTests.AppControllersTests
{
    internal class LayoutControllerTest : BaseTestController
    {
        [Test]
        public async Task CreateLayout_VenueManagerRole_ReturnsCreateLayoutPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Layout/CreateLayout?venueId=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Create layout");
        }

        [Test]
        public async Task CreateLayout_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Layout/CreateLayout";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "Test description" },
                { "VenueId", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task CreateLayout_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Layout/CreateLayout";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "First layout" },
                { "VenueId", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("The same layout is already exists in current venue.");
        }

        [Test]
        public async Task EditLayout_VenueManagerRole_ReturnsEditLayoutPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Layout/EditLayout?id=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Edit layout");
        }

        [Test]
        public async Task EditLayout_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Layout/EditLayout";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "1" },
                { "Description", "Test description" },
                { "VenueId", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task EditLayout_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Layout/EditLayout";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "1" },
                { "Description", "Second layout" },
                { "VenueId", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("The same layout is already exists in current venue.");
        }

        [Test]
        public async Task DeleteLayout_ExistingLayout_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Layout/DeleteLayout?id=2";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task DeleteLayout_LayoutHostingEvent_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Layout/DeleteLayout?id=1";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("This layout cannot be deleted as it will host an event.");
        }
    }
}
