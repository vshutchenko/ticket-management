using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.EventServiceTests
{
    internal class UpdateEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;

        [SetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var eventRepo = new EventSqlClientRepository(connectionString);
            var areaRepo = new AreaSqlClientRepository(connectionString);
            var seatRepo = new SeatSqlClientRepository(connectionString);

            var eventAreaRepo = new EventAreaSqlClientRepository(connectionString);
            var eventSeatRepo = new EventSeatSqlClientRepository(connectionString);

            var eventValidator = new EventValidator(eventRepo, seatRepo, areaRepo);
            var priceValidator = new PriceValidator();

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _eventSeatService = new EventSeatService(eventSeatRepo, eventAreaRepo, mapper);
            _eventAreaService = new EventAreaService(eventAreaRepo, eventRepo, priceValidator, mapper);

            _eventService = new EventService(eventRepo, eventValidator, _eventSeatService, _eventAreaService, mapper);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesEvent()
        {
            // Arrange
            var id = 1;

            var expectedEventBeforeUpdate = new EventModel
            {
                Id = id,
                Name = "First event",
                Description = "First event description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                EndDate = new DateTime(2023, 1, 1, 15, 0, 0),
                ImageUrl = "url",
                Published = true,
            };

            var actualEventBeforeUpdate = await _eventService.GetByIdAsync(id);

            actualEventBeforeUpdate.Should().BeEquivalentTo(expectedEventBeforeUpdate);

            var eventToUpdate = new EventModel
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
                ImageUrl = "new url",
                Published = true,
            };

            // Act
            await _eventService.UpdateAsync(eventToUpdate);

            var actualEvent = await _eventService.GetByIdAsync(id);

            // Assert
            actualEvent.Should().BeEquivalentTo(eventToUpdate);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesAreas()
        {
            // Arrange
            var id = 1;

            var expectedEventAreasBeforeUpdate = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "Event area of first event", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var actualEventAreasBeforeUpdate = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            actualEventAreasBeforeUpdate.Should().BeEquivalentTo(expectedEventAreasBeforeUpdate);

            var expectedEventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 2, Description = "First area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
                new EventAreaModel { Id = 3, Description = "Second area of first layout", CoordX = 1, CoordY = 1, EventId = 1, Price = 0 },
            };

            var eventToUpdate = new EventModel
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
                ImageUrl = "url",
                Published = true,
            };

            // Act
            await _eventService.UpdateAsync(eventToUpdate);

            var actualEventAreas = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            // Assert
            actualEventAreas.Should().BeEquivalentTo(expectedEventAreas);
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesSeats()
        {
            // Arrange
            var id = 1;

            var expectedEventSeatsBeforeUpdate = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 4, EventAreaId = 1, Row = 2, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 5, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Available },
            };

            var actualEventSeatsBeforeUpdate = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            actualEventSeatsBeforeUpdate.Should().BeEquivalentTo(expectedEventSeatsBeforeUpdate);

            var expectedEventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 6, EventAreaId = 2, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 7, EventAreaId = 2, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 8, EventAreaId = 2, Row = 1, Number = 3, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 9, EventAreaId = 2, Row = 2, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 10, EventAreaId = 2, Row = 2, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 11, EventAreaId = 3, Row = 1, Number = 1, State = EventSeatStateModel.Available },
            };

            var eventToUpdate = new EventModel
            {
                Id = id,
                Name = "First Updated Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
                ImageUrl = "url",
                Published = true,
            };

            // Act
            await _eventService.UpdateAsync(eventToUpdate);

            var actualEventSeats = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            // Assert
            actualEventSeats.Should().BeEquivalentTo(expectedEventSeats);
        }
    }
}
