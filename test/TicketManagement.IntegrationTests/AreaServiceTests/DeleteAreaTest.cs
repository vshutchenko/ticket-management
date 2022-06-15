using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

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
            _areaService.DeleteAsync(id);

            _areaService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
