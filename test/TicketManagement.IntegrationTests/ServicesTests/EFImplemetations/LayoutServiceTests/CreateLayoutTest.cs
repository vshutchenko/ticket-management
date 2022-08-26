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

namespace TicketManagement.IntegrationTests.EFImplemetations.LayoutServiceTests
{
    internal class CreateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var layoutRepo = new LayoutRepository(context);
            var venueRepo = new VenueRepository(context);
            var eventRepo = new EventRepository(context);
            var layoutValidator = new LayoutValidator(layoutRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _layoutService = new LayoutService(layoutRepo, venueRepo, eventRepo, layoutValidator, mapper);
        }

        [Test]
        public async Task Create_ValidLayout_CreatesLayout()
        {
            // Arrange
            var layoutToCreate = new LayoutModel
            {
                 Description = "Test layout 1",
                 VenueId = 2,
            };

            // Act
            var id = await _layoutService.CreateAsync(layoutToCreate);

            var actualLayout = await _layoutService.GetByIdAsync(id);

            // Assert
            actualLayout.Should().BeEquivalentTo(layoutToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
