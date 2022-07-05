using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [SetUp]
        public void SetUp()
        {
            _eventRepositoryMock = new Mock<IRepository<Event>>();
            _seatRepositoryMock = new Mock<IRepository<Seat>>();
            _areaRepositoryMock = new Mock<IRepository<Area>>();

            var eventValidator = new EventValidator(_eventRepositoryMock.Object, _seatRepositoryMock.Object, _areaRepositoryMock.Object);

            _eventService = new EventService(_eventRepositoryMock.Object, eventValidator);
        }

        [Test]
        public void Create_ValidEvent_CreatesEvent()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 10, 10),
                EndDate = new DateTime(2023, 10, 11),
            };

            _eventService.CreateAsync(eventToCreate);

            _eventRepositoryMock.Verify(x => x.CreateAsync(eventToCreate), Times.Once);
        }

        [Test]
        public void Create_EventInThePast_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Invoking(s => s.CreateAsync(eventToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Start date is in the past.");
        }

        [Test]
        public void Create_InvalidStartDate_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Invoking(s => s.CreateAsync(eventToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public void Create_EventInTheSameLayoutExists_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.CreateAsync(eventToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
        }

        [Test]
        public void Create_NoAvailableSeats_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>().AsQueryable());

            var eventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 3,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.CreateAsync(eventToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public void Create_NullEvent_ThrowsValidationException()
        {
            _eventService.Invoking(s => s.CreateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public void Delete_EventExists_DeletesEvent()
        {
            int id = 1;

            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _eventService.DeleteAsync(id);

            _eventRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public void Update_ValidEvent_UpdatesEvent()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            int id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "Updated Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.UpdateAsync(eventToUpdate);

            _eventRepositoryMock.Verify(x => x.UpdateAsync(eventToUpdate), Times.Once);
        }

        [Test]
        public void Update_EventInThePast_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            int id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Invoking(s => s.UpdateAsync(eventToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Start date is in the past.");
        }

        [Test]
        public void Update_InvalidStartDate_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            int id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
            };

            _eventService.Invoking(s => s.UpdateAsync(eventToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public void Update_EventInTheSameLayoutAlreadyExists_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            int id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.UpdateAsync(eventToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
        }

        [Test]
        public void Update_NoAvailableSeats_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            int id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>().AsQueryable());

            var eventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 3,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 2),
            };

            _eventService.Invoking(s => s.UpdateAsync(eventToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public void Update_NullEvent_ThrowsValidationException()
        {
            _eventService.Invoking(s => s.UpdateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public void GetAll_EventListNotEmpty_ReturnsEventList()
        {
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) },
                new Event { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
                new Event { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5) },
            };

            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _eventService.GetAll().Should().BeEquivalentTo(events);
        }

        [Test]
        public async Task GetById_EventExists_ReturnsEvent()
        {
            int id = 1;
            var @event = new Event { Id = 1, Name = "New Event", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 10, 10), EndDate = new DateTime(2023, 10, 11) };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);

            var actualEvent = await _eventService.GetByIdAsync(id);

            actualEvent.Should().BeEquivalentTo(@event);
        }

        [Test]
        public void GetById_InvalidId_ThrowsValidationException()
        {
            int id = 1;

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<Event>(null);

            _eventService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
