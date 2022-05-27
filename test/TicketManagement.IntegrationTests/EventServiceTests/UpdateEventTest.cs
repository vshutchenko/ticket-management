using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.EventServiceTests
{
    internal class UpdateEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;
        private IAreaService _areaService;
        private ISeatService _seatService;

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
            var seatValidationService = new SeatValidator(seatRepo);
            var areaValidationService = new AreaValidator(areaRepo);
            var priceValidationService = new PriceValidator();

            _seatService = new SeatService(seatRepo, seatValidationService);
            _areaService = new AreaService(areaRepo, areaValidationService);
            _eventService = new EventService(eventRepo, eventValidationService);

            _eventSeatService = new EventSeatService(eventSeatRepo);
            _eventAreaService = new EventAreaService(eventAreaRepo, priceValidationService);
        }

        [Test]
        public void Update_ValidEvent_Event_EventAreas_EventSeats_AreUpdated()
        {
            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "First Updated Event",
                Descpription = "Test description",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 2),
                EndDate = new DateTime(2023, 1, 3),
            };

            var expectedEventAreasCount = _areaService
                .GetAll()
                .Count(a => a.LayoutId == eventToUpdate.LayoutId);

            var expectedEventSeatsCount = _areaService
                .GetAll()
                .Where(a => a.LayoutId == eventToUpdate.LayoutId)
                .Sum(a => _seatService.GetAll().Count(s => s.AreaId == a.Id));

            _eventService.Update(eventToUpdate);

            var actualEventAreasCount = _eventAreaService
                .GetAll()
                .Count(a => a.EventId == eventToUpdate.Id);

            var actualEventSeatsCount = _eventAreaService
                .GetAll()
                .Where(a => a.EventId == eventToUpdate.Id)
                .Sum(a => _eventSeatService.GetAll().Count(s => s.EventAreaId == a.Id));

            Assert.AreEqual(expectedEventAreasCount, actualEventAreasCount);
            Assert.AreEqual(expectedEventSeatsCount, actualEventSeatsCount);
        }
    }
}
