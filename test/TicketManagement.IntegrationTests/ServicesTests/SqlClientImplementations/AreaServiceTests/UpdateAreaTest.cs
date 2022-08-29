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

namespace TicketManagement.IntegrationTests.SqlClientImplementations.AreaServiceTests
{
    internal class UpdateAreaTest
    {
        private IAreaService _areaService;

        [SetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var areaRepo = new AreaSqlClientRepository(connectionString);
            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var areaValidator = new AreaValidator(areaRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _areaService = new AreaService(areaRepo, layoutRepo, areaValidator, mapper);
        }

        [Test]
        public async Task UpdateDescription_ValidArea_UpdatesArea()
        {
            // Arrange
            var id = 1;

            var areaBeforeUpdate = new AreaModel
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new AreaModel
            {
                Id = id,
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            // Act
            await _areaService.UpdateAsync(areaToUpdate);

            var actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            // Assert
            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }

        [Test]
        public async Task UpdateCoordinates_ValidArea_UpdatesArea()
        {
            // Arrange
            var id = 1;

            var areaBeforeUpdate = new AreaModel
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new AreaModel
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 6,
                CoordY = 6,
                LayoutId = 1,
            };

            // Act
            await _areaService.UpdateAsync(areaToUpdate);

            var actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            // Assert
            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }
    }
}
