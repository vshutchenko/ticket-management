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
    internal class CreateAreaTest
    {
        private IAreaService _areaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            AreaSqlClientRepository areaRepo = new AreaSqlClientRepository(connectionString);
            AreaValidator areaValidator = new AreaValidator(areaRepo);

            _areaService = new AreaService(areaRepo, areaValidator);
        }

        [Test]
        public async Task Create_ValidArea_CreatesArea()
        {
            Area areaToCreate = new Area
            {
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            var id = await _areaService.CreateAsync(areaToCreate);

            var actualArea = await _areaService.GetByIdAsync(id);

            actualArea.Should().BeEquivalentTo(areaToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
