using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.AreaServiceTests
{
    internal class CreateAreaTest
    {
        private IAreaService _areaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var areaRepo = new AreaSqlClientRepository(connectionString);
            var areaValidator = new AreaValidator(areaRepo);

            _areaService = new AreaService(areaRepo, areaValidator);
        }

        [Test]
        public void Create_ValidArea_CreatesArea()
        {
            var areaToCreate = new Area
            {
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var id = _areaService.Create(areaToCreate);

            _areaService.GetById(id).Should().BeEquivalentTo(areaToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
