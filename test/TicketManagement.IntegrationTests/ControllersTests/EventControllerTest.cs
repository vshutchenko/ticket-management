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
    internal sealed class EventControllerTest : IDisposable
    {
        private readonly TestingWebAppFactory _factory = new TestingWebAppFactory();

        [Test]
        public async Task Index_AnonymousUser_ReturnsEventListPage()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Event";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("List of events");
        }

        [Test]
        public async Task Index_UserRole_ReturnsEventListPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("List of events");
        }

        [Test]
        public async Task Index_EventManagerRole_ReturnsEventListPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("List of events");
        }

        [Test]
        public async Task CreateEvent_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Event/CreateEvent";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Sign in");
        }

        [Test]
        public async Task CreateEvent_UserRole_ReturnsForbiddenResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event/CreateEvent";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task CreateEvent_EventManagerRole_ReturnsCreateEventPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event/CreateEvent";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Create event");
        }

        [Test]
        public async Task CreateEvent_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Event/CreateEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Name", "First Event" },
                { "Description", "Test description" },
                { "Layout", "1" },
                { "StartDate", "01.01.2023, 12:00" },
                { "EndDate", "02.01.2023, 10:00" },
                { "ImageUrl", "url" },
                { "Published", "false" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task CreateEvent_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Event/CreateEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Name", "First Event" },
                { "Description", "Test description" },
                { "Layout", "1" },
                { "StartDate", "02.01.2023, 12:00" },
                { "EndDate", "01.01.2023, 10:00" },
                { "ImageUrl", "url" },
                { "Published", "false" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("End date is less than start date.");
        }

        [Test]
        public async Task EditEvent_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/Event/EditEvent?id=1";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Sign in");
        }

        [Test]
        public async Task EditEvent_UserRole_ReturnsForbiddenResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event/EditEvent?id=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task EditEvent_EventManagerRole_ReturnsOkResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/Event/EditEvent?id=1";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task EditPublishedEvent_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Event/EditPublishedEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Event/EditEvent?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Event.Id", "1" },
                { "Event.Name", "First Event" },
                { "Event.Description", "Test description" },
                { "Event.Layout", "1" },
                { "Event.StartDate", "01.01.2023, 12:00" },
                { "Event.EndDate", "02.01.2023, 10:00" },
                { "Event.ImageUrl", "url" },
                { "Event.Published", "true" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task EditPublishedEvent_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Event/EditPublishedEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Event/EditEvent?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Event.Id", "1" },
                { "Event.Name", "First Event" },
                { "Event.Description", "Test description" },
                { "Event.Layout", "1" },
                { "Event.StartDate", "02.01.2023, 12:00" },
                { "Event.EndDate", "01.01.2023, 10:00" },
                { "Event.ImageUrl", "url" },
                { "Event.Published", "true" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("End date is less than start date.");
        }

        [Test]
        public async Task EditNotPublishedEvent_ValidModel_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Event/EditNotPublishedEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Event/EditEvent?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Event.Id", "1" },
                { "Event.Name", "First Event" },
                { "Event.Description", "Test description" },
                { "Event.Layout", "1" },
                { "Event.StartDate", "01.01.2023, 12:00" },
                { "Event.EndDate", "02.01.2023, 10:00" },
                { "Event.ImageUrl", "url" },
                { "Event.Published", "true" },
                { "Areas[0].Id", "1" },
                { "Areas[0].Price", "12" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Test]
        public async Task EditNotPublishedEvent_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Event/EditNotPublishedEvent";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync("/Event/EditEvent?id=1");
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Event.Id", "1" },
                { "Event.Name", "First Event" },
                { "Event.Description", "Test description" },
                { "Event.Layout", "1" },
                { "Event.StartDate", "02.01.2023, 12:00" },
                { "Event.EndDate", "01.01.2023, 10:00" },
                { "Event.ImageUrl", "url" },
                { "Event.Published", "true" },
                { "Areas[0].Id", "1" },
                { "Areas[0].Price", "12" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("End date is less than start date.");
        }

        [Test]
        public async Task DeleteEvent_ExistingEvent_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Event/DeleteEvent?id=1";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
