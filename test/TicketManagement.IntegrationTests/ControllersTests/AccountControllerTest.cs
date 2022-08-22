﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using RestEase;
using TicketManagement.IntegrationTests.ControllersTests.Addition;
using TicketManagement.WebApplication.Clients.EventApi;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal sealed class AccountControllerTest : BaseTestController
    {
        [Test]
        public async Task Login_AnonymousUser_ReturnsLoginPage()
        {
            // Arrange
            var client = AppFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authoriation", "Bearer sfsdpkofpsd");

            // Act
            var response = await client.GetAsync("/Account/Login");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Sign in");
        }

        [Test]
        public async Task Login_ValidModel_ReturnsEventListPage()
        {
            // Arrange
            var url = "/Account/Login";
            var client = AppFactory.CreateClient();

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Email", "user1@gmail.com" },
                { "Password", "Password123#" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("List of events");
        }

        [Test]
        public async Task Login_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Account/Login";
            var client = AppFactory.CreateClient();

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var notExistingEmail = "test@mail.ru";

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "Email", notExistingEmail },
                { "Password", "1234567890" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("User with such email does not exists.");
        }

        [Test]
        public async Task Register_AnonymousUser_ReturnsRegisterPage()
        {
            // Arrange
            var client = AppFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/Account/Register");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("Register");
        }

        [Test]
        public async Task Register_ValidModel_ReturnsEventListPage()
        {
            // Arrange
            var url = "/Account/Register";
            var client = AppFactory.CreateClient();

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "FirstName", "John" },
                { "LastName", "Doe" },
                { "CultureName", "en-US" },
                { "TimeZoneId", TimeZoneInfo.Local.Id },
                { "Email", "newUser@gmail.com" },
                { "Password", "1234567890" },
                { "ConfirmPassword", "1234567890" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            responseString.Should().Contain("List of events");
        }

        [Test]
        public async Task Register_InvalidModel_ReturnsOkResultWithErrorMessage()
        {
            // Arrange
            var url = "/Account/Register";
            var client = AppFactory.CreateClient();

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var alreadyTakenEmail = "user1@gmail.com";

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "FirstName", "John" },
                { "LastName", "Doe" },
                { "CultureName", "en-US" },
                { "TimeZoneId", TimeZoneInfo.Local.Id },
                { "Email", alreadyTakenEmail },
                { "Password", "1234567890" },
                { "ConfirmPassword", "1234567890" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString.Should().Contain("User already exists.");
        }

        [Test]
        public async Task AddFunds_EventManagerRole_ReturnsForbiddenResult()
        {
            // Arrange
            var url = "/Account/AddFunds";
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task ChangePassword_UserRole_ReturnsOkResult()
        {
            // Arrange
            var url = "/Account/ChangePassword";
            var provider = TestClaimsProvider.WithUserClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_EventManagerRole_ReturnsOkResult()
        {
            // Arrange
            var url = "/Account/ChangePassword";
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task ChangePassword_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var url = "/Account/ChangePassword";
            var provider = TestClaimsProvider.WithUserClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            var getResponse = await client.GetAsync(url);
            var antiForgery = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(getResponse);

            var formModel = new Dictionary<string, string>
            {
                { AntiForgeryTokenExtractor.Field, antiForgery.field },
                { "CurrentPassword", "Password123#" },
                { "NewPassword", "NewPassword" },
                { "ConfirmNewPassword", "NewPassword" },
            };

            // Act
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formModel));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task EditUser_UserRole_ReturnsOkResult()
        {
            // Arrange
            var url = "/Account/EditUser?userId=d33655d7-af47-49c7-a004-64969e5b651f";
            var provider = TestClaimsProvider.WithUserClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task EditUser_EventManagerRole_ReturnsOkResult()
        {
            // Arrange
            var url = "/Account/EditUser?userId=d33655d7-af47-49c7-a004-64969e5b651f";
            var provider = TestClaimsProvider.WithEventManagerClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Logout_AuthenticatedUser_ReturnsRedirectResult()
        {
            // Arrange
            var url = "/Account/Logout";
            var provider = TestClaimsProvider.WithUserClaims();
            var client = AppFactory.CreateClientWithTestAuth(provider);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }
    }
}
