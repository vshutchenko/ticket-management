using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.VenueServiceTests
{
    internal class DeleteVenueTest
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

        [TestCase(1)]
        public void Delete_VenueExists_DeletesVenue(int id)
        {
            _venueService.Delete(id);

            _venueService.Invoking(s => s.GetById(id))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
