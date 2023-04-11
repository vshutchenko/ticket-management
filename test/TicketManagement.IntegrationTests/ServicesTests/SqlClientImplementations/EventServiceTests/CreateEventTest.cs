using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.Core.Models;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.EventApi.MappingConfig;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.EventServiceTests
{
    internal class CreateEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;

        [SetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

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
                })
                .CreateMapper();

            _eventSeatService = new EventSeatService(eventSeatRepo, eventAreaRepo, mapper);
            _eventAreaService = new EventAreaService(eventAreaRepo, eventRepo, priceValidator, mapper);

            _eventService = new EventService(eventRepo, eventValidator, _eventSeatService, _eventAreaService, mapper);
        }

        [Test]
        public async Task Create_ValidEvent_CreatesEvent()
        {
            // Arrange
            var eventToCreate = new EventModel
            {
                Id = 2,
                Name = "First Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 3),
                ImageUrl = "url",
                Published = false,
            };

            // Act
            var id = await _eventService.CreateAsync(eventToCreate);

            var actualEvent = await _eventService.GetByIdAsync(id);

            // Assert
            actualEvent.Should().BeEquivalentTo(eventToCreate, opt => opt.Excluding(e => e.Id));
        }

        [Test]
        public async Task Create_ValidEvent_CreatesAreas()
        {
            // Arrange
            var eventToCreate = new EventModel
            {
                Id = 2,
                Name = "First Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 3),
                ImageUrl = "url",
                Published = false,
            };

            var expectedEventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 2, Description = "First area of first layout", CoordX = 1, CoordY = 1, EventId = 2, Price = 0 },
                new EventAreaModel { Id = 3, Description = "Second area of first layout", CoordX = 1, CoordY = 1, EventId = 2, Price = 0 },
            };

            // Act
            var id = await _eventService.CreateAsync(eventToCreate);

            var actualEventAreas = _eventAreaService.GetAll().Where(a => a.EventId == id).ToList();

            // Assert
            actualEventAreas.Should().BeEquivalentTo(expectedEventAreas);
        }

        [Test]
        public async Task Create_ValidEvent_CreatesSeats()
        {
            // Arrange
            var eventToCreate = new EventModel
            {
                Name = "First Event",
                Description = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 3),
                ImageUrl = "url",
                Published = false,
            };

            var expectedEventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 6, EventAreaId = 2, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeatModel { Id = 7, EventAreaId = 2, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeatModel { Id = 8, EventAreaId = 2, Row = 1, Number = 3, State = EventSeatState.Available },
                new EventSeatModel { Id = 9, EventAreaId = 2, Row = 2, Number = 1, State = EventSeatState.Available },
                new EventSeatModel { Id = 10, EventAreaId = 2, Row = 2, Number = 2, State = EventSeatState.Available },
                new EventSeatModel { Id = 11, EventAreaId = 3, Row = 1, Number = 1, State = EventSeatState.Available },
            };

            // Act
            var id = await _eventService.CreateAsync(eventToCreate);

            var actualEventSeats = _eventAreaService.GetAll()
                .Where(a => a.EventId == id)
                .Join(_eventSeatService.GetAll(), a => a.Id, s => s.EventAreaId, (a, s) => s)
                .ToList();

            // Assert
            actualEventSeats.Should().BeEquivalentTo(expectedEventSeats);
        }
    }
}
