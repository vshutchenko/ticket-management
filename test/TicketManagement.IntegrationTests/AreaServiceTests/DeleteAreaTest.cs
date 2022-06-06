using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.AreaServiceTests
{
    internal class DeleteAreaTest
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

        [TestCase(1)]
        public void Delete_AreaExists_DeletesArea(int id)
        {
            _areaService.Delete(id);

            Assert.IsNull(_areaService.GetById(id));
        }
    }
}
