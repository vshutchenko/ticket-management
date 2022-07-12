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
        public async Task Update_ValidEvent_UpdatesEvent()
        {
            int id = 1;

            Event expectedEventBeforeUpdate = new Event
            {
                Id = id,
                Name = "First event",
                Description = "First event description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                EndDate = new DateTime(2023, 1, 1, 15, 0, 0),
            };

            BusinessLogic.Models.EventModel actualEventBeforeUpdate = await _eventService.GetByIdAsync(id);

            actualEventBeforeUpdate.Should().BeEquivalentTo(expectedEventBeforeUpdate);

            Event eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            BusinessLogic.Models.EventModel actualEvent = await _eventService.GetByIdAsync(id);

            actualEvent.Should().BeEquivalentTo(eventToUpdate);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesAreas()
        {
            int id = 1;

            List<EventArea> expectedEventAreasBeforeUpdate = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Event area of first event", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            List<BusinessLogic.Models.EventAreaModel> actualEventAreasBeforeUpdate = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreasBeforeUpdate.Should().BeEquivalentTo(expectedEventAreasBeforeUpdate);

            List<EventArea> expectedEventAreas = new List<EventArea>
            {
                new EventArea { Id = 2, Description = "First area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
                new EventArea { Id = 3, Description = "Second area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
            };

            Event eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            List<BusinessLogic.Models.EventAreaModel> actualEventAreas = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreas.Should().BeEquivalentTo(expectedEventAreas);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesSeats()
        {
            int id = 1;

            List<EventSeat> expectedEventSeatsBeforeUpdate = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Ordered },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Ordered },
                new EventSeat { Id = 3, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
                new EventSeat { Id = 4, EventAreaId = 1, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 5, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatState.Available },
            };

            List<BusinessLogic.Models.EventSeatModel> actualEventSeatsBeforeUpdate = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeatsBeforeUpdate.Should().BeEquivalentTo(expectedEventSeatsBeforeUpdate);

            List<EventSeat> expectedEventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 6, EventAreaId = 2, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 7, EventAreaId = 2, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 8, EventAreaId = 2, Row = 1, Number = 3, State = EventSeatState.Available },
                new EventSeat { Id = 9, EventAreaId = 2, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 10, EventAreaId = 2, Row = 2, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 11, EventAreaId = 3, Row = 1, Number = 1, State = EventSeatState.Available },
            };

            Event eventToUpdate = new Event
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            await _eventService.UpdateAsync(eventToUpdate);

            List<BusinessLogic.Models.EventSeatModel> actualEventSeats = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeats.Should().BeEquivalentTo(expectedEventSeats);
        }
    }
}
