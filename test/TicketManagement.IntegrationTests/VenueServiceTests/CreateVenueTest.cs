using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.VenueServiceTests
{
    internal class CreateVenueTest
    {
        private IVenueService _venueService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            VenueSqlClientRepository venueRepo = new VenueSqlClientRepository(connectionString);
            VenueValidator venueValidator = new VenueValidator(venueRepo);

            _venueService = new VenueService(venueRepo, venueValidator);
        }

        [Test]
        public async Task Create_ValidVenue_CreatesVenue()
        {
            Venue venueToCreate = new Venue
            {
                Description = "Test venue 1",
                Address = "Address 1-22",
                Phone = "880055535335",
            };

            var id = await _venueService.CreateAsync(venueToCreate);

            var actualVenue = await _venueService.GetByIdAsync(id);

            actualVenue.Should().BeEquivalentTo(venueToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
