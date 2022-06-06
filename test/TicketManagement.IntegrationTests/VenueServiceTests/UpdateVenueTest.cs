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

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var venueRepo = new VenueSqlClientRepository(connectionString);
            var venueValidator = new VenueValidator(venueRepo);

            _venueService = new VenueService(venueRepo, venueValidator);
        }

        [Test]
        public void Update_ValidVenue_UpdatesVenue()
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
                Description = "Updated venue 1",
                Address = "Address 1-22",
                Phone = "880055535335",
            };

            _venueService.Update(venueToUpdate);

            _venueService.GetById(id).Should().BeEquivalentTo(venueToUpdate);
        }
    }
}
