using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.EventAreaServiceTests
{
    internal class EventAreaServiceTest
    {
        private IEventAreaService _eventAreaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            EventAreaSqlClientRepository eventareaRepo = new EventAreaSqlClientRepository(connectionString);

            _eventAreaService = new EventAreaService(eventareaRepo, new PriceValidator());
        }

        [Test]
        public async Task SetPrice_ValidPrice_SetsPrice()
        {
            int id = 1;

            decimal priceBeforeUpdate = 15;

            BusinessLogic.Models.EventAreaModel eventAreaBeforeUpdate = await _eventAreaService.GetByIdAsync(id);

            eventAreaBeforeUpdate.Price.Should().Be(priceBeforeUpdate);

            decimal priceToUpdate = 10;

            await _eventAreaService.SetPriceAsync(id, priceToUpdate);

            BusinessLogic.Models.EventAreaModel eventAreaAfterUpdate = await _eventAreaService.GetByIdAsync(id);

            eventAreaAfterUpdate.Price.Should().Be(priceToUpdate);
        }
    }
}
