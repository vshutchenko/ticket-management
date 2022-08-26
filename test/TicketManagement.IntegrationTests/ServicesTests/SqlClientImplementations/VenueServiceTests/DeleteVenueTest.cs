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
        public async Task Delete_VenueHostsEvent_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            // Act
            var deletingVenue = _venueService.Invoking(s => s.DeleteAsync(id));

            // Assert
            await deletingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("This venue cannot be deleted as it will host an event.");
        }

        [Test]
        public async Task Delete_VenueExists_DeletesVenue()
        {
            // Arrange
            var id = 2;

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
