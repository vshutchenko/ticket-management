﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.LayoutServiceTests
{
    internal class DeleteLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var venueRepo = new VenueSqlClientRepository(connectionString);
            var eventRepo = new EventSqlClientRepository(connectionString);
            var layoutValidator = new LayoutValidator(layoutRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _layoutService = new LayoutService(layoutRepo, venueRepo, eventRepo, layoutValidator, mapper);
        }

        [Test]
        public async Task Delete_LayoutHostsEvent_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            // Act
            var deletingLayout = _layoutService.Invoking(s => s.DeleteAsync(id));

            // Assert
            await deletingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("This layout cannot be deleted as it will host an event.");
        }

        [Test]
        public async Task Delete_LayoutExists_DeletesLayout()
        {
            // Arrange
            var id = 2;

            // Act
            await _layoutService.DeleteAsync(id);

            var gettingLayout = _layoutService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
