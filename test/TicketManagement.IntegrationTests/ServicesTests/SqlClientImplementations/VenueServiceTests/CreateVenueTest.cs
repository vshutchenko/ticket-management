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

namespace TicketManagement.IntegrationTests.SqlClientImplementations.VenueServiceTests
{
    internal class CreateVenueTest
    {
        private IVenueService _venueService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var venueRepo = new VenueSqlClientRepository(connectionString);
            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var eventRepo = new EventSqlClientRepository(connectionString);
            var venueValidator = new VenueValidator(venueRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _venueService = new VenueService(venueRepo, layoutRepo, eventRepo, venueValidator, mapper);
        }

        [Test]
        public async Task Create_ValidVenue_CreatesVenue()
        {
            // Arrange
            var venueToCreate = new VenueModel
            {
                Description = "Test venue 1",
                Address = "Address 1-22",
                Phone = "880055535335",
            };

            // Act
            var id = await _venueService.CreateAsync(venueToCreate);

            var actualVenue = await _venueService.GetByIdAsync(id);

            // Assert
            actualVenue.Should().BeEquivalentTo(venueToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
