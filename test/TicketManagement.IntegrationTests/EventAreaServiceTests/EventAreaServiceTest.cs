using FluentAssertions;
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

        [Test]
        public void SetPrice_ValidPrice_SetsPrice()
        {
            int id = 1;

            decimal priceBeforeUpdate = 15;

            _eventAreaService.GetById(id).Price.Should().Be(priceBeforeUpdate);

            decimal priceToUpdate = 10;

            _eventAreaService.SetPrice(id, priceToUpdate);

            _eventAreaService.GetById(id).Price.Should().Be(priceToUpdate);
        }
    }
}
