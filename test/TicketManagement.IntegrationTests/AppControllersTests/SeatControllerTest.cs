using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.Addition;

namespace TicketManagement.IntegrationTests.AppControllersTests
{
    internal class SeatControllerTest : BaseTestController
    {
        [Test]
        public async Task AddSeatRow_VenueManagerRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);
            var url = "/Seat/AddSeatRow?areaId=1&row=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task AddSeatRow_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Seat/AddSeatRow";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "AreaId", "1" },
                { "Row", "10" },
                { "Length", "12" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task AddSeatRow_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Seat/AddSeatRow";

            var provider = TestClaimsProvider.WithVenueManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Account/Login");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "AreaId", "99" },
                { "Row", "1" },
                { "Length", "12" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Entity was not found.");
        }

        [Test]
        public async Task DeleteSeatRow_ExistingArea_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Seat/DeleteSeatRow?areaId=1&row=1";

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
