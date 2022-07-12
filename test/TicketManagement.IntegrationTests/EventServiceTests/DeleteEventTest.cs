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
            string connectionString = new TestDatabase().ConnectionString;

            EventSqlClientRepository eventRepo = new EventSqlClientRepository(connectionString);
            AreaSqlClientRepository areaRepo = new AreaSqlClientRepository(connectionString);
            SeatSqlClientRepository seatRepo = new SeatSqlClientRepository(connectionString);

            EventAreaSqlClientRepository eventAreaRepo = new EventAreaSqlClientRepository(connectionString);
            EventSeatSqlClientRepository eventSeatRepo = new EventSeatSqlClientRepository(connectionString);

            EventValidator eventValidationService = new EventValidator(eventRepo, seatRepo, areaRepo);
            PriceValidator priceValidationService = new PriceValidator();

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

            int actualEventAreasCount = _eventAreaService
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

            int actualEventSeatsCount = _eventAreaService
                .GetAll()
                .Where(a => a.EventId == id)
                .Sum(a => _eventSeatService.GetAll().Count(s => s.EventAreaId == a.Id));

            expectedEventSeatsCount.Should().Be(actualEventSeatsCount);
        }
    }
}
