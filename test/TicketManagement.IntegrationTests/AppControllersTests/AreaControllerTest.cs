using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.Addition;

namespace TicketManagement.IntegrationTests.AppControllersTests
{
    internal class AreaControllerTest : BaseTestController
    {
        [Test]
        public async Task AreaList_VenueManagerRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Area/AreaList?layoutId=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task CreateArea_VenueManagerRole_ReturnsCreateAreaPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Area/CreateArea?layoutId=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Create area");
        }

        [Test]
        public async Task CreateArea_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Area/CreateArea";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "Test description" },
                { "LayoutId", "1" },
                { "CoordX", "1" },
                { "CoordY", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task CreateArea_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Area/CreateArea";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Description", "First area of first layout" },
                { "LayoutId", "1" },
                { "CoordX", "1" },
                { "CoordY", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Area description should be unique in the layout.");
        }

        [Test]
        public async Task EditArea_VenueManagerRole_ReturnsEditAreaPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Area/EditArea?id=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Edit area");
        }

        [Test]
        public async Task EditArea_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Area/EditArea";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "1" },
                { "Description", "Test description" },
                { "LayoutId", "1" },
                { "CoordX", "1" },
                { "CoordY", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task EditArea_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Area/EditArea";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Id", "2" },
                { "Description", "First area of first layout" },
                { "LayoutId", "1" },
                { "CoordX", "1" },
                { "CoordY", "1" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Area description should be unique in the layout.");
        }

        [Test]
        public async Task DeleteArea_ExistingArea_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Area/DeleteArea?id=1";

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
    }
}
