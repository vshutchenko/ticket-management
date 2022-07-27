using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.IntegrationTests.ControllersTests.Addition;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal sealed class EventImportControllerTest : IDisposable
    {
        private readonly TestingWebAppFactory _factory = new TestingWebAppFactory();

        [Test]
        public async Task ImportEvents_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "/EventImport/ImportEvents";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Sign in");
        }

        [Test]
        public async Task ImportEvents_UserRole_ReturnsForbiddenResult()
        {
            // Arrange
            var provider = TestClaimsProvider.WithUserClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/EventImport/ImportEvents";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task ImportEvents_EventManagerRole_ReturnsImportEventsPage()
        {
            // Arrange
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var url = "/EventImport/ImportEvents";

            // Act
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Import events");
        }

        [Test]
        public async Task ImportEvents_ValidModel_ReturnsImportDetails()
        {
            // Arrange
            var url = "/EventImport/ImportEvents";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var filePath = Path.Combine(Environment.CurrentDirectory, "Files", "events.json");

            var httpContent = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            httpContent.Add(fileContent, "EventsJson", filePath);

            httpContent.Add(new StringContent("1"), "Layout");

            httpContent.Add(new StringContent(antiForgery.field), AntiForgeryTokenExtractor.Field);

            // Act
            var response = await client.PostAsync(url, httpContent);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("All events were successfully imported!");
        }

        [Test]
        public async Task ImportEvents_InvalidModel_ReturnsImportDetailsWithErrorMessage()
        {
            // Arrange
            var url = "/EventImport/ImportEvents";

            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = _factory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var filePath = Path.Combine(Environment.CurrentDirectory, "Files", "invalidEvents.json");

            var httpContent = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            httpContent.Add(fileContent, "EventsJson", filePath);

            httpContent.Add(new StringContent("1"), "Layout");

            httpContent.Add(new StringContent(antiForgery.field), AntiForgeryTokenExtractor.Field);

            // Act
            var response = await client.PostAsync(url, httpContent);

            // Assert
            var responseString = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("End date is less than start date.");
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
