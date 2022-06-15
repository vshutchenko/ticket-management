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
    internal class UpdateEventTest
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
        public async Task Update_ValidEvent_UpdatesEvent()
        {
            int id = 1;

            var expectedEventBeforeUpdate = new Event
            {
                Id = id,
                Name = "First event",
                Descpription = "First event description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                EndDate = new DateTime(2023, 1, 1, 15, 0, 0),
            };

            var actualEventBeforeUpdate = await _eventService.GetByIdAsync(id);

            actualEventBeforeUpdate.Should().BeEquivalentTo(expectedEventBeforeUpdate);

            var eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            var actualEvent = await _eventService.GetByIdAsync(id);

            actualEvent.Should().BeEquivalentTo(eventToUpdate);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesAreas()
        {
            int id = 1;

            var expectedEventAreasBeforeUpdate = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Event area of first event", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var actualEventAreasBeforeUpdate = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreasBeforeUpdate.Should().BeEquivalentTo(expectedEventAreasBeforeUpdate);

            var expectedEventAreas = new List<EventArea>
            {
                new EventArea { Id = 2, Description = "First area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
                new EventArea { Id = 3, Description = "Second area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
            };

            var eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            var actualEventAreas = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreas.Should().BeEquivalentTo(expectedEventAreas);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesSeats()
        {
            int id = 1;

            var expectedEventSeatsBeforeUpdate = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Ordered },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Ordered },
                new EventSeat { Id = 3, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
                new EventSeat { Id = 4, EventAreaId = 1, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 5, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatState.Available },
            };

            var actualEventSeatsBeforeUpdate = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeatsBeforeUpdate.Should().BeEquivalentTo(expectedEventSeatsBeforeUpdate);

            var expectedEventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 6, EventAreaId = 2, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 7, EventAreaId = 2, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 8, EventAreaId = 2, Row = 1, Number = 3, State = EventSeatState.Available },
                new EventSeat { Id = 9, EventAreaId = 2, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 10, EventAreaId = 2, Row = 2, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 11, EventAreaId = 3, Row = 1, Number = 1, State = EventSeatState.Available },
            };

            var eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            var actualEventSeats = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeats.Should().BeEquivalentTo(expectedEventSeats);
        }
    }
}
