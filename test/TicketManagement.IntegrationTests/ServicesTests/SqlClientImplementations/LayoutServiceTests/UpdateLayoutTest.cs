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

namespace TicketManagement.IntegrationTests.SqlClientImplementations.LayoutServiceTests
{
    internal class UpdateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var venueRepo = new VenueSqlClientRepository(connectionString);
            var eventRepo = new EventSqlClientRepository(connectionString);
            var layoutValidator = new LayoutValidator(layoutRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _layoutService = new LayoutService(layoutRepo, venueRepo, eventRepo, layoutValidator, mapper);
        }

        [Test]
        public async Task Update_ValidLayout_UpdatesLayout()
        {
            // Arrange
            var id = 1;

            var layoutBeforeUpdate = new LayoutModel
            {
                Id = id,
                Description = "First layout",
                VenueId = 1,
            };

            var actualLayoutBeforeUpdate = await _layoutService.GetByIdAsync(id);

            actualLayoutBeforeUpdate.Should().BeEquivalentTo(layoutBeforeUpdate);

            var layoutToUpdate = new LayoutModel
            {
                Id = id,
                Description = "Test layout 1",
                VenueId = 1,
            };

            // Act
            await _layoutService.UpdateAsync(layoutToUpdate);

            var layoutAfterUpdate = await _layoutService.GetByIdAsync(id);

            // Assert
            layoutAfterUpdate.Should().BeEquivalentTo(layoutToUpdate);
        }
    }
}
