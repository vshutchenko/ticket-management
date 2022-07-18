using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.ControllersTests.Addition;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal sealed class PurchaseControllerTest : IDisposable
    {
        private readonly TestingWebAppFactory _factory = new TestingWebAppFactory();

        [Test]
        public async Task PurchaseHistory_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = _factory.CreateClient();

            var url = "/Purchase/PurchaseHistory";

            // Act
            var response = await client.GetAsync(url);

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Contains("Sign in");
        }

        [Test]
        public async Task PurchaseHistory_EventManagerRole_ReturnsForbiddenResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Purchase/PurchaseHistory";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task PurchaseHistory_UserRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Purchase/PurchaseHistory";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PurchaseSeats_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = _factory.CreateClient();

            var url = "/Purchase/PurchaseSeats?id=1";

            // Act
            var response = await client.GetAsync(url);

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Contains("Sign in");
        }

        [Test]
        public async Task PurchaseSeats_EventManagerRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Purchase/PurchaseSeats?id=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PurchaseSeats_UserRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Purchase/PurchaseSeats?id=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task PurchaseSeats_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Purchase/PurchaseSeats?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "EventId", "1" },
                { "UserId", "d33655d7-af47-49c7-a004-64969e5b651f" },
                { "SeatIds[0]", "1" },
                { "SeatIds[1]", "2" },
            };

            // Act
            var response = await client.PostAsync("/Purchase/PurchaseSeats", new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task PurchaseSeats_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Purchase/PurchaseSeats?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var notExistingSeatId = "99";

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "EventId", "1" },
                { "UserId", "d33655d7-af47-49c7-a004-64969e5b651f" },
                { "SeatIds[0]", notExistingSeatId },
            };

            // Act
            var response = await client.PostAsync("/Purchase/PurchaseSeats", new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("Entity was not found.");
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
