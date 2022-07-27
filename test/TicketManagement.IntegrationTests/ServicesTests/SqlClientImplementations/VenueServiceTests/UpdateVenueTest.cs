﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.VenueServiceTests
{
    internal class UpdateVenueTest
    {
        private IVenueService _venueService;

        [SetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var venueRepo = new VenueSqlClientRepository(connectionString);
            var venueValidator = new VenueValidator(venueRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _venueService = new VenueService(venueRepo, venueValidator, mapper);
        }

        [Test]
        public async Task UpdateDescription_ValidVenue_UpdatesVenue()
        {
            // Arrange
            var id = 1;

            var venueBeforeUpdate = new VenueModel
            {
                Id = id,
                Description = "First venue",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            var actualVenueBeforeUpdate = await _venueService.GetByIdAsync(id);

            actualVenueBeforeUpdate.Should().BeEquivalentTo(venueBeforeUpdate);

            var venueToUpdate = new VenueModel
            {
                Id = id,
                Description = "Test venue 1",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            // Act
            await _venueService.UpdateAsync(venueToUpdate);

            var actualVenueAfterUpdate = await _venueService.GetByIdAsync(id);

            // Assert
            actualVenueAfterUpdate.Should().BeEquivalentTo(venueToUpdate);
        }

        [Test]
        public async Task UpdateAddressAndPhone_ValidVenue_UpdatesVenue()
        {
            // Arrange
            var id = 1;

            var venueBeforeUpdate = new VenueModel
            {
                Id = id,
                Description = "First venue",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            var actualVenueBeforeUpdate = await _venueService.GetByIdAsync(id);

            actualVenueBeforeUpdate.Should().BeEquivalentTo(venueBeforeUpdate);

            var venueToUpdate = new VenueModel
            {
                Id = id,
                Description = "First venue",
                Address = "Test address",
                Phone = "111 11 678 11 12",
            };

            // Act
            await _venueService.UpdateAsync(venueToUpdate);

            var actualVenueAfterUpdate = await _venueService.GetByIdAsync(id);

            // Assert
            actualVenueAfterUpdate.Should().BeEquivalentTo(venueToUpdate);
        }
    }
}