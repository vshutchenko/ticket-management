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
            string connectionString = new TestDatabase().ConnectionString;

            AreaSqlClientRepository areaRepo = new AreaSqlClientRepository(connectionString);
            AreaValidator areaValidator = new AreaValidator(areaRepo);

            _areaService = new AreaService(areaRepo, areaValidator);
        }

        [Test]
        public async Task UpdateDescription_ValidArea_UpdatesArea()
        {
            int id = 1;

            Area areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            BusinessLogic.Models.AreaModel actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            Area areaToUpdate = new Area
            {
                Id = id,
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            await _areaService.UpdateAsync(areaToUpdate);

            BusinessLogic.Models.AreaModel actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }

        [Test]
        public async Task UpdateCoordinates_ValidArea_UpdatesArea()
        {
            int id = 1;

            Area areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            BusinessLogic.Models.AreaModel actualAreaBeforeUpdate = await _areaService.GetByIdAsync(id);

            actualAreaBeforeUpdate.Should().BeEquivalentTo(areaBeforeUpdate);

            Area areaToUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 6,
                CoordY = 6,
                LayoutId = 1,
            };

            await _areaService.UpdateAsync(areaToUpdate);

            BusinessLogic.Models.AreaModel actualAreaAfterUpdate = await _areaService.GetByIdAsync(id);

            actualAreaAfterUpdate.Should().BeEquivalentTo(areaToUpdate);
        }
    }
}
