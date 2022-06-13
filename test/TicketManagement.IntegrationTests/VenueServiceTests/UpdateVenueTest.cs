using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.VenueServiceTests
{
    internal class UpdateVenueTest
    {
        private IVenueService _venueService;

        [SetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var venueRepo = new VenueSqlClientRepository(connectionString);
            var venueValidator = new VenueValidator(venueRepo);

            _venueService = new VenueService(venueRepo, venueValidator);
        }

        [Test]
        public void UpdateDescription_ValidVenue_UpdatesVenue()
        {
            int id = 1;

            var venueBeforeUpdate = new Venue
            {
                Id = id,
                Description = "First venue",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            _venueService.GetById(id).Should().BeEquivalentTo(venueBeforeUpdate);

            var venueToUpdate = new Venue
            {
                Id = id,
                Description = "Test venue 1",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            _venueService.Update(venueToUpdate);

            _venueService.GetById(id).Should().BeEquivalentTo(venueToUpdate);
        }

        [Test]
        public void UpdateAddressAndPhone_ValidVenue_UpdatesVenue()
        {
            int id = 1;

            var venueBeforeUpdate = new Venue
            {
                Id = id,
                Description = "First venue",
                Address = "First venue address",
                Phone = "123 45 678 90 12",
            };

            _venueService.GetById(id).Should().BeEquivalentTo(venueBeforeUpdate);

            var venueToUpdate = new Venue
            {
                Id = id,
                Description = "First venue",
                Address = "Test address",
                Phone = "111 11 678 11 12",
            };

            _venueService.Update(venueToUpdate);

            _venueService.GetById(id).Should().BeEquivalentTo(venueToUpdate);
        }
    }
}
