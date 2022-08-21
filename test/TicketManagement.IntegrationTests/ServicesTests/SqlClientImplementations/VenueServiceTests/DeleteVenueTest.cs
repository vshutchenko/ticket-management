using System.Threading.Tasks;
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
    internal class DeleteVenueTest
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
                })
                .CreateMapper();

            _venueService = new VenueService(venueRepo, venueValidator, mapper);
        }

        public async Task Delete_VenueExists_DeletesVenue()
        {
            // Arrange
            var id = 1;

            // Act
            await _venueService.DeleteAsync(id);

            var gettingById = _venueService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
