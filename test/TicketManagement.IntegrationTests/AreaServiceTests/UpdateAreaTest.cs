using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

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
        public void UpdateDescription_ValidArea_UpdatesArea()
        {
            int id = 1;

            var areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            _areaService.GetById(id).Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new Area
            {
                Id = id,
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            _areaService.Update(areaToUpdate);

            _areaService.GetById(id).Should().BeEquivalentTo(areaToUpdate);
        }

        [Test]
        public void UpdateCoordinates_ValidArea_UpdatesArea()
        {
            int id = 1;

            var areaBeforeUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            _areaService.GetById(id).Should().BeEquivalentTo(areaBeforeUpdate);

            var areaToUpdate = new Area
            {
                Id = id,
                Description = "First area of first layout",
                CoordX = 6,
                CoordY = 6,
                LayoutId = 1,
            };

            _areaService.Update(areaToUpdate);

            _areaService.GetById(id).Should().BeEquivalentTo(areaToUpdate);
        }
    }
}
