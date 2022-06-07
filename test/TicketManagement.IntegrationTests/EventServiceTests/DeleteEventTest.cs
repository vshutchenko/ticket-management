using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.EventServiceTests
{
    internal class DeleteEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var eventRepo = new EventSqlClientRepository(connectionString);
            var areaRepo = new AreaSqlClientRepository(connectionString);
            var seatRepo = new SeatSqlClientRepository(connectionString);

            var eventAreaRepo = new EventAreaSqlClientRepository(connectionString);
            var eventSeatRepo = new EventSeatSqlClientRepository(connectionString);

            var eventValidationService = new EventValidator(eventRepo, seatRepo, areaRepo);
            var priceValidationService = new PriceValidator();

            _eventService = new EventService(eventRepo, eventValidationService);

            _eventSeatService = new EventSeatService(eventSeatRepo);
            _eventAreaService = new EventAreaService(eventAreaRepo, priceValidationService);
        }

        [TestCase(1)]
        public void Delete_EventExists_DeletesEvent(int id)
        {
            int expectedEventAreasCount = 0;
            int expectedEventSeatsCount = 0;

            _eventService.Delete(id);

            var actualEventAreasCount = _eventAreaService
                .GetAll()
                .Count(a => a.EventId == id);

            var actualEventSeatsCount = _eventAreaService
                .GetAll()
                .Where(a => a.EventId == id)
                .Sum(a => _eventSeatService.GetAll().Count(s => s.EventAreaId == a.Id));

            expectedEventAreasCount.Should().Be(actualEventAreasCount);
            expectedEventSeatsCount.Should().Be(actualEventSeatsCount);

            _eventService.GetById(id).Should().BeNull();
        }
    }
}
