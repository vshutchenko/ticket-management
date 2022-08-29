using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.Addition;

namespace TicketManagement.IntegrationTests.AppControllersTests
{
    internal class VenueControllerTest : BaseTestController
    {
        [Test]
        public async Task VenueList_VenueManagerRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Venue/VenueList";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task CreateVenue_VenueManagerRole_ReturnsCreateVenuePage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Venue/CreateVenue";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Create venue");
        }

        [Test]
        public async Task CreateVenue_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Venue/CreateVenue";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "Test description" },
                { "Address", "Test address" },
                { "Phone", "123124234" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task CreateVenue_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Venue/CreateVenue";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "First venue" },
                { "Address", "Test address" },
                { "Phone", "123124234" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Venue with same description is already exists.");
        }

        [Test]
        public async Task EditVenue_VenueManagerRole_ReturnsEditVenuePage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Venue/EditVenue?id=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Edit venue");
        }

        [Test]
        public async Task EditVenue_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Venue/EditVenue";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "1" },
                { "Description", "Test description" },
                { "Address", "Test address" },
                { "Phone", "123124234" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task EditVenue_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Venue/EditVenue";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "1" },
                { "Description", "Second venue" },
                { "Address", "Test address" },
                { "Phone", "123124234" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Venue with same description is already exists.");
        }

        [Test]
        public async Task DeleteVenue_ExistingVenue_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Venue/DeleteVenue?id=2";

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
        public async Task DeleteVenue_VenueHostingEvent_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Venue/DeleteVenue?id=1";

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
            responseString.Should().Contain("This venue cannot be deleted as it will host an event.");
        }
    }
}
