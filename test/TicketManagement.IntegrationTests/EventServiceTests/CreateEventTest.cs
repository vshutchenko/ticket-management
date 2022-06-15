using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.EventServiceTests
{
    internal class CreateEventTest
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
        public async Task Create_ValidEvent_CreatesEvent()
        {
            var eventToCreate = new Event
            {
                Id = 2,
                Name = "First Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            var id = await _eventService.CreateAsync(eventToCreate);

            var actualEvent = await _eventService.GetByIdAsync(id);

            actualEvent.Should().BeEquivalentTo(eventToCreate, opt => opt.Excluding(e => e.Id));
        }

        [Test]
        public async Task Create_ValidEvent_CreatesAreas()
        {
            var eventToCreate = new Event
            {
                Id = 2,
                Name = "First Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            var expectedEventAreas = new List<EventArea>
            {
                new EventArea { Id = 2, Description = "First area of first layout", CoordX = 1, CoordY = 1, EventId = 2, Price = 0 },
                new EventArea { Id = 3, Description = "Second area of first layout", CoordX = 1, CoordY = 1, EventId = 2, Price = 0 },
            };

            var id = await _eventService.CreateAsync(eventToCreate);

            var actualEventAreas = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreas.Should().BeEquivalentTo(expectedEventAreas);
        }

        [Test]
        public async Task Create_ValidEvent_CreatesSeats()
        {
            var eventToCreate = new Event
            {
                Name = "First Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            var expectedEventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 6, EventAreaId = 2, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 7, EventAreaId = 2, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 8, EventAreaId = 2, Row = 1, Number = 3, State = EventSeatState.Available },
                new EventSeat { Id = 9, EventAreaId = 2, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 10, EventAreaId = 2, Row = 2, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 11, EventAreaId = 3, Row = 1, Number = 1, State = EventSeatState.Available },
            };

            var id = await _eventService.CreateAsync(eventToCreate);

            List<EventSeat> actualEventSeats = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeats.Should().BeEquivalentTo(expectedEventSeats);
        }
    }
}
