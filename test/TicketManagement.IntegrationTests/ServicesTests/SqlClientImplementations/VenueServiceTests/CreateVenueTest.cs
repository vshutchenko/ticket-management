using System.Threading.Tasks;
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
    internal class CreateVenueTest
    {
        private IVenueService _venueService;

        [OneTimeSetUp]
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
