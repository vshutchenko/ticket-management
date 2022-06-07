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
        public void Create_ValidEvent_CreatesEvent()
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

            _eventService.Invoking(s => s.Create(eventToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("Start date is in the past.");
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

            _eventService.Invoking(s => s.Create(eventToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public void Create_EventInTheSameLayoutExists_ThrowsValidationException()
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

            _eventService.Invoking(s => s.Create(eventToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
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
                LayoutId = 3,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.Create(eventToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public void Create_NullEvent_ThrowsValidationException()
        {
            _eventService.Invoking(s => s.Create(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public void Delete_EventExists_DeletesEvent()
        {
            _eventService.Delete(It.IsAny<int>());

            _eventRepositoryMock.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Update_ValidEvent_UpdatesEvent()
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
        public void Update_EventInThePast_ThrowsValidationException()
        {
            var eventToUpdate = new Event
            {
                Id = 2,
                Name = "New Event",
                Descpription = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Invoking(s => s.Update(eventToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Start date is in the past.");
        }

        [Test]
        public void Update_InvalidStartDate_ThrowsValidationException()
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

            _eventService.Invoking(s => s.Update(eventToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public void Update_EventInTheSameLayoutIsAlreadyExist_ThrowsValidationException()
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

            _eventService.Invoking(s => s.Update(eventToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
        }

        [Test]
        public void Update_NoAvailableSeats_ThrowsValidationException()
        {
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Descpription = "Description 1",
                LayoutId = 3,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.Update(eventToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public void Update_NullEvent_ThrowsValidationException()
        {
            _eventService.Invoking(s => s.Update(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public void GetAll_EventListNotEmpty_ReturnsEventList()
        {
            var events = _eventService.GetAll();

            events.Should().BeEquivalentTo(_events);
        }

        [Test]
        public void GetById_EventExists_ReturnsEvent()
        {
            var @event = new Event { Id = 1, Name = "New Event", Descpription = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 10, 10), EndDate = new DateTime(2023, 10, 11) };

            _eventRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(@event);

            _eventService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(@event);
        }

        [Test]
        public void GetById_InvalidId_ReturnsNUll()
        {
            _eventRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Event>(null);

            _eventService.GetById(It.IsAny<int>()).Should().BeNull();
        }
    }
}
