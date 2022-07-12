using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
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

            _areaService = new AreaService(areaRepo, areaValidator);
        }

        [Test]
        public async Task UpdateDescription_ValidArea_UpdatesArea()
        {
            var id = 1;

            var areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new Area
            {
                Id = id,
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            await _areaService.UpdateAsync(areaToUpdate);

            var actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }

        [Test]
        public async Task UpdateCoordinates_ValidArea_UpdatesArea()
        {
            var id = 1;

            var areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 6,
                CoordY = 6,
                LayoutId = 1,
            };

            await _areaService.UpdateAsync(areaToUpdate);

            var actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }
    }
}
