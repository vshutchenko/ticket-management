using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.EFImplemetations.VenueServiceTests
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

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var venueRepo = new VenueRepository(context);
            var layoutRepo = new LayoutRepository(context);
            var eventRepo = new EventRepository(context);
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
