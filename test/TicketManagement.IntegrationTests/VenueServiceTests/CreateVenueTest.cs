﻿using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.VenueServiceTests
{
    internal class CreateVenueTest
    {
        private IVenueService _venueService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var venueRepo = new VenueSqlClientRepository(connectionString);
            var venueValidator = new VenueValidator(venueRepo);

            _venueService = new VenueService(venueRepo, venueValidator);
        }

        [Test]
        public void Create_ValidVenue_CreatesVenue()
        {
            var venueToCreate = new Venue
            {
                Description = "Test venue 1",
                Address = "Address 1-22",
                Phone = "880055535335",
            };

            var id = _venueService.Create(venueToCreate);

            _venueService.GetById(id).Should().BeEquivalentTo(venueToCreate, v => v.Excluding(v => v.Id));
        }
    }
}