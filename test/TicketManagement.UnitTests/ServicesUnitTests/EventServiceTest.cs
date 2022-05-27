using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class EventServiceTest
    {
        private Mock<IRepository<Event>> _eventRepositoryMock;
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private IEventService _eventService;
        private List<Event> _events;
        private List<Area> _areas;
        private List<Seat> _seats;

        [SetUp]
        public void SetUp()
        {
            _events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Descpription = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) },
                new Event { Id = 1, Name = "Event 2", Descpription = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) },
                new Event { Id = 1, Name = "Event 3", Descpription = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5) },
            };

            _areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            _seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 2, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 2, },
            };

            _eventRepositoryMock = new Mock<IRepository<Event>>();
            _seatRepositoryMock = new Mock<IRepository<Seat>>();
            _areaRepositoryMock = new Mock<IRepository<Area>>();

            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(_events);
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(_seats);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(_areas);

            var eventValidator = new EventValidator(_eventRepositoryMock.Object, _seatRepositoryMock.Object, _areaRepositoryMock.Object);

            _eventService = new EventService(_eventRepositoryMock.Object, eventValidator);
        }

        [Test]
        public void Create_ValidEvent_EventCreated()
        {
            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 10, 10),
                EndDate = new DateTime(2023, 10, 11),
            };

            _eventService.Create(eventToCreate);

            _eventRepositoryMock.Verify(x => x.Create(eventToCreate), Times.Once);
        }

        [Test]
        public void Create_EventInThePast_ThrowsValidationException()
        {
            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Create(eventToCreate));
        }

        [Test]
        public void Create_InvalidStartDate_ThrowsValidationException()
        {
            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Create(eventToCreate));
        }

        [Test]
        public void Create_EventInTheSameLayoutIsAlreadyExist_ThrowsValidationException()
        {
            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Create(eventToCreate));
        }

        [Test]
        public void Create_NoAvailableSeats_ThrowsValidationException()
        {
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Create(eventToCreate));
        }

        [Test]
        public void Create_NullEvent_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _eventService.Create(null));
        }

        [Test]
        public void Delete_EventDeleted()
        {
            _eventService.Delete(It.IsAny<int>());

            _eventRepositoryMock.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Update_ValidEvent_EventUpdated()
        {
            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "Updated Event",
                Descpription = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Update(eventToUpdate);

            _eventRepositoryMock.Verify(x => x.Update(It.IsAny<Event>()), Times.Once);
        }

        [Test]
        public void Update_EventInThePast_ThrowsValidationExceptiont()
        {
            var eventToCreate = new Event
            {
                Id = 2,
                Name = "New Event",
                Descpription = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Create(eventToCreate));
        }

        [Test]
        public void Update_InvalidStartDate_ThrowsValidationExceptiont()
        {
            var eventToUpdate = new Event
            {
                Id = 2,
                Name = "New Event",
                Descpription = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Update(eventToUpdate));
        }

        [Test]
        public void Update_EventInTheSameLayoutIsAlreadyExist_ThrowsValidationExceptiont()
        {
            var eventToUpdate = new Event
            {
                Id = 2,
                Name = "New Event",
                Descpription = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            Assert.Throws<ValidationException>(() => _eventService.Update(eventToUpdate));
        }

        [Test]
        public void Update_NullEvent_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _eventService.Update(null));
        }

        [Test]
        public void GetAll_EventListReturned()
        {
            var events = _eventService.GetAll();

            events.Should().BeEquivalentTo(_events);
        }

        [Test]
        public void GetById_Test()
        {
            var @event = new Event { Id = 1, Name = "New Event", Descpription = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 10, 10), EndDate = new DateTime(2023, 10, 11) };

            _eventRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(@event);

            _eventService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(@event);
        }

        [Test]
        public void GetById_InvalidId_ThrowsArgumentException()
        {
            _eventRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Event>(null);

            Assert.IsNull(_eventService.GetById(It.IsAny<int>()));
        }
    }
}
