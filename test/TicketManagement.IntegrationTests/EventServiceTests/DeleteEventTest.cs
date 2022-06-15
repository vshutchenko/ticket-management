using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.EventServiceTests
{
    internal class DeleteEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;

        [SetUp]
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

        [Test]
        public async Task Delete_EventExists_DeletesEvent()
        {
            int id = 1;

            await _eventService.DeleteAsync(id);

            await _eventService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Delete_EventExists_DeletesAreas()
        {
            int id = 1;
            int expectedEventAreasCount = 0;

            await _eventService.DeleteAsync(id);

            var actualEventAreasCount = _eventAreaService
                .GetAll()
                .Count(a => a.EventId == id);

            actualEventAreasCount.Should().Be(expectedEventAreasCount);
        }

        [Test]
        public async Task Delete_EventExists_DeletesSeats()
        {
            int id = 1;
            int expectedEventSeatsCount = 0;

            await _eventService.DeleteAsync(id);

            var actualEventSeatsCount = _eventAreaService
                .GetAll()
                .Where(a => a.EventId == id)
                .Sum(a => _eventSeatService.GetAll().Count(s => s.EventAreaId == a.Id));

            expectedEventSeatsCount.Should().Be(actualEventSeatsCount);
        }
    }
}
