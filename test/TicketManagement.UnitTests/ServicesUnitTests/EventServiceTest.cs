using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TicketManagement.Core.Models;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class EventServiceTest
    {
        private Mock<IRepository<Event>> _eventRepositoryMock;
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private IEventService _eventService;
        private Mock<IEventAreaService> _eventAreaServiceMock;
        private Mock<IEventSeatService> _eventSeatServiceMock;

        [SetUp]
        public void SetUp()
        {
            _eventRepositoryMock = new Mock<IRepository<Event>>();
            _seatRepositoryMock = new Mock<IRepository<Seat>>();
            _areaRepositoryMock = new Mock<IRepository<Area>>();
            _eventAreaServiceMock = new Mock<IEventAreaService>();
            _eventSeatServiceMock = new Mock<IEventSeatService>();
            _mapperMock = new Mock<IMapper>();

            var eventValidator = new EventValidator(_eventRepositoryMock.Object, _seatRepositoryMock.Object, _areaRepositoryMock.Object);

            _eventService = new EventService(_eventRepositoryMock.Object, eventValidator, _eventSeatServiceMock.Object, _eventAreaServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidEvent_CreatesEvent()
        {
            // Arrange
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

            var eventToCreate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 10, 10),
                EndDate = new DateTime(2023, 10, 11),
            };

            var mappedEventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(2023, 10, 10),
                EndDate = new DateTime(2023, 10, 11),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToCreate)).Returns(mappedEventToCreate);

            // Act
            await _eventService.CreateAsync(eventToCreate);

            // Assert
            _eventRepositoryMock.Verify(x => x.CreateAsync(mappedEventToCreate), Times.Once);
        }

        [Test]
        public async Task Create_EventInThePast_ThrowsValidationException()
        {
            // Arrange
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

            var eventToCreate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToCreate)).Returns(mappedEventToCreate);

            // Act
            var creatingEvent = _eventService.Invoking(s => s.CreateAsync(eventToCreate));

            // Assert
            await creatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Start date is in the past.");
        }

        [Test]
        public async Task Create_InvalidStartDate_ThrowsValidationException()
        {
            // Arrange
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

            var eventToCreate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 2, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 2, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToCreate)).Returns(mappedEventToCreate);

            // Act
            var creatingEvent = _eventService.Invoking(s => s.CreateAsync(eventToCreate));

            // Assert
            await creatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public async Task Create_EventInTheSameLayoutExists_ThrowsValidationException()
        {
            // Arrange
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
                new Event
                {
                    Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1), EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
                },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            var eventToCreate = new EventModel
            {
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToCreate = new Event
            {
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToCreate)).Returns(mappedEventToCreate);

            // Act
            var creatingEvent = _eventService.Invoking(s => s.CreateAsync(eventToCreate));

            // Assert
            await creatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
        }

        [Test]
        public async Task Create_NoAvailableSeats_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>().AsQueryable());

            var eventToCreate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToCreate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToCreate)).Returns(mappedEventToCreate);

            // Act
            var creatingEvent = _eventService.Invoking(s => s.CreateAsync(eventToCreate));

            // Assert
            await creatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public async Task Create_NullEvent_ThrowsValidationException()
        {
            // Arrange
            EventModel nullEvent = null;

            // Act
            var creratingEvent = _eventService.Invoking(s => s.CreateAsync(nullEvent));

            // Assert
            await creratingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public async Task Delete_EventExists_DeletesEvent()
        {
            // Arrange
            var id = 1;

            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            // Act
            await _eventService.DeleteAsync(id);

            // Assert
            _eventRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_EventSeatOrdered_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
                new EventSeatModel { Id = 2, Row = 1, Number = 2, EventAreaId = 1, State = EventSeatState.Ordered },
            };

            var id = 1;

            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            // Act
            var deletingEvent = _eventService.Invoking(s => s.DeleteAsync(id));

            await deletingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event cannot be deleted because some seats has already been purchased.");
        }

        [Test]
        public async Task Update_ValidEvent_UpdatesEvent()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
            };

            var id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 1,
                Name = "Updated Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 1,
                Name = "Updated Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            await _eventService.UpdateAsync(eventToUpdate);

            // Assert
            _eventRepositoryMock.Verify(x => x.UpdateAsync(mappedEventToUpdate), Times.Once);
        }

        [Test]
        public async Task Update_EventSeatOrdered_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
                new EventSeatModel { Id = 2, Row = 1, Number = 2, EventAreaId = 1, State = EventSeatState.Ordered },
            };

            var id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 1,
                Name = "Updated Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 1,
                Name = "Updated Event",
                Description = "Description 1",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            var updatingEvent = _eventService.Invoking(s => s.UpdateAsync(eventToUpdate));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Some seats have already been ordered.");
        }

        [Test]
        public async Task Update_EventInThePast_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
            };

            var id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            var updatingEvent = _eventService.Invoking(s => s.UpdateAsync(eventToUpdate));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Start date is in the past.");
        }

        [Test]
        public async Task Update_InvalidStartDate_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
            };

            var id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 2, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 2, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            var updatingEvent = _eventService.Invoking(s => s.UpdateAsync(eventToUpdate));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("End date is less than start date.");
        }

        [Test]
        public async Task Update_EventInTheSameLayoutAlreadyExists_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
            };

            var id = 2;
            var events = new List<Event>
            {
                new Event
                {
                    Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1), EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
                },
                new Event
                {
                    Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 1, StartDate = new DateTime(DateTime.Now.Year + 2, 1, 1), EndDate = new DateTime(DateTime.Now.Year + 2, 1, 2),
                },
            };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(events.First(e => e.Id == id));
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 2,
                Name = "Updated Event",
                Description = "Updated Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 2,
                Name = "Updated Event",
                Description = "Updated Description",
                LayoutId = 1,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            var updaingEvent = _eventService.Invoking(s => s.UpdateAsync(eventToUpdate));

            // Assert
            await updaingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event in the same layout and the same time is already exists.");
        }

        [Test]
        public async Task Update_NoAvailableSeats_ThrowsValidationException()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            var eventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "EventArea 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 15 },
            };

            var eventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, Row = 1, Number = 1, EventAreaId = 1, State = EventSeatState.Available },
            };

            var id = 1;
            var @event = new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) };
            var events = new List<Event> { @event };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);
            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Seat>().AsQueryable());

            _eventAreaServiceMock.Setup(x => x.GetByEventId(id)).Returns(eventAreas);

            foreach (var area in eventAreas)
            {
                _eventSeatServiceMock.Setup(x => x.GetByEventAreaId(area.Id))
                    .Returns(eventSeats.Where(s => s.EventAreaId == area.Id));
            }

            var eventToUpdate = new EventModel
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 3,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            var mappedEventToUpdate = new Event
            {
                Id = 1,
                Name = "New Event",
                Description = "Description 1",
                LayoutId = 3,
                StartDate = new DateTime(DateTime.Now.Year + 1, 1, 1),
                EndDate = new DateTime(DateTime.Now.Year + 1, 1, 2),
            };

            _mapperMock.Setup(m => m.Map<Event>(eventToUpdate)).Returns(mappedEventToUpdate);

            // Act
            var updatingEvent = _eventService.Invoking(s => s.UpdateAsync(eventToUpdate));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("There are no available seats in the layout.");
        }

        [Test]
        public async Task Update_NullEvent_ThrowsValidationException()
        {
            // Arrange
            EventModel nullEvent = null;

            // Act
            var updatingEvent = _eventService.Invoking(s => s.UpdateAsync(nullEvent));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Event is null.");
        }

        [Test]
        public void GetPublishedEvents_EventListNotEmpty_ReturnsEventList()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2), Published = true },
                new Event { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
                new Event { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5), Published = true },
            };

            var mappedEvents = new List<EventModel>
            {
                new EventModel { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2), Published = true },
                new EventModel { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
                new EventModel { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5), Published = true },
            };

            var expectedEvents = new List<EventModel>
            {
                new EventModel { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2), Published = true },
                new EventModel { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5), Published = true },
            };

            for (var i = 0; i < events.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventModel>(events[i])).Returns(mappedEvents[i]);
            }

            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            // Act
            var actualEvents = _eventService.GetAll(EventFilter.Published);

            // Assert
            actualEvents.Should().BeEquivalentTo(expectedEvents);
        }

        [Test]
        public void GetNotPublishedEvents_EventListNotEmpty_ReturnsEventList()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2), Published = true },
                new Event { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
                new Event { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5), Published = true },
            };

            var mappedEvents = new List<EventModel>
            {
                new EventModel { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2), Published = true },
                new EventModel { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
                new EventModel { Id = 3, Name = "Event 3", Description = "Description 3", LayoutId = 2, StartDate = new DateTime(2023, 1, 3), EndDate = new DateTime(2023, 1, 5), Published = true },
            };

            var expectedEvents = new List<EventModel>
            {
                new EventModel { Id = 2, Name = "Event 2", Description = "Description 2", LayoutId = 2, StartDate = new DateTime(2023, 2, 2, 10, 0, 0), EndDate = new DateTime(2023, 2, 2, 15, 0, 0) },
            };

            for (var i = 0; i < events.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventModel>(events[i])).Returns(mappedEvents[i]);
            }

            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());

            // Act
            var actualEvents = _eventService.GetAll(EventFilter.NotPublished);

            // Assert
            actualEvents.Should().BeEquivalentTo(expectedEvents);
        }

        [Test]
        public async Task GetById_EventExists_ReturnsEvent()
        {
            // Arrange
            var id = 1;
            var @event = new Event { Id = 1, Name = "New Event", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 10, 10), EndDate = new DateTime(2023, 10, 11) };
            var mappedEvent = new EventModel { Id = 1, Name = "New Event", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 10, 10), EndDate = new DateTime(2023, 10, 11) };

            _mapperMock.Setup(m => m.Map<EventModel>(@event)).Returns(mappedEvent);

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(@event);

            // Act
            var actualEvent = await _eventService.GetByIdAsync(id);

            // Assert
            actualEvent.Should().BeEquivalentTo(@event);
        }

        [Test]
        public async Task GetById_InvalidId_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Event));

            // Act
            var gettingById = _eventService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
