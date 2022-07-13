using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.AreaServiceTests
{
    internal class UpdateAreaTest
    {
        private IAreaService _areaService;

        [SetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var areaRepo = new AreaSqlClientRepository(connectionString);
            var areaValidator = new AreaValidator(areaRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _areaService = new AreaService(areaRepo, areaValidator, mapper);
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
