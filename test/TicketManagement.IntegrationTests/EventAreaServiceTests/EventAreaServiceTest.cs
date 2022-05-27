using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.EventAreaServiceTests
{
    internal class EventAreaServiceTest
    {
        private IEventAreaService _eventAreaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var eventareaRepo = new EventAreaSqlClientRepository(connectionString);

            _eventAreaService = new EventAreaService(eventareaRepo, new PriceValidator());
        }

        [TestCase(1, 10)]
        public void SetPrice_ValidPrice_PriceChanged(int id, decimal price)
        {
            _eventAreaService.SetPrice(id, price);

            var actualPrice = _eventAreaService.GetById(id).Price;

            Assert.AreEqual(price, actualPrice);
        }
    }
}
