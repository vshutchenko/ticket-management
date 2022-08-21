using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class EventSeatServiceTest
    {
        private Mock<IRepository<EventSeat>> _eventSeatRepositoryMock;
        private Mock<IRepository<EventArea>> _eventAreaRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private IEventSeatService _eventSeatService;

        [SetUp]
        public void SetUp()
        {
            _eventSeatRepositoryMock = new Mock<IRepository<EventSeat>>();
            _eventAreaRepositoryMock = new Mock<IRepository<EventArea>>();
            _mapperMock = new Mock<IMapper>();

            _eventSeatService = new EventSeatService(_eventSeatRepositoryMock.Object, _eventAreaRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task SetSeatState_ValidParameters_SetsState()
        {
            // Arrange
            var id = 1;

            var eventSeat = new EventSeat { Id = 1, EventAreaId = 1, Number = 1, Row = 1, State = EventSeatState.Available };

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventSeat);

            // Act
            await _eventSeatService.SetSeatStateAsync(id, EventSeatStateModel.Ordered);

            // Assert
            _eventSeatRepositoryMock.Verify(x => x.UpdateAsync(eventSeat), Times.Once);
        }

        [Test]
        public async Task SetSeatState_SeatNotFound_ThrowsValidationException()
        {
            // Arrange
            var notExistingId = 1;

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(EventSeat));

            // Act
            var settingState = _eventSeatService.Invoking(s => s.SetSeatStateAsync(notExistingId, EventSeatStateModel.Ordered));

            // Assert
            await settingState
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void GetAll_EventSeatListNotEmpty_ReturnsEventSeatList()
        {
            // Arrange
            var eventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
            };

            var mappedEventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatStateModel.Ordered },
            };

            for (var i = 0; i < eventSeats.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventSeatModel>(eventSeats[i])).Returns(mappedEventSeats[i]);
            }

            _eventSeatRepositoryMock.Setup(x => x.GetAll()).Returns(eventSeats.AsQueryable());

            // Act
            var actualSeats = _eventSeatService.GetAll();

            // Assert
            actualSeats.Should().BeEquivalentTo(eventSeats);
        }

        [Test]
        public void GetByEventAreaId_EventSeatListNotEmpty_ReturnsEventSeatList()
        {
            // Arrange
            var eventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
            };

            var mappedEventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatStateModel.Ordered },
            };

            var id = 1;
            var eventAreas = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
            };

            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas.AsQueryable());
            _eventSeatRepositoryMock.Setup(x => x.GetAll()).Returns(eventSeats.AsQueryable());

            for (var i = 0; i < eventSeats.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventSeatModel>(eventSeats[i])).Returns(mappedEventSeats[i]);
            }

            // Act
            var actualSeats = _eventSeatService.GetByEventAreaId(id);

            // Assert
            actualSeats.Should().BeEquivalentTo(mappedEventSeats);
        }

        [Test]
        public void GetByEventAreaId_EventAreaNotFound_ThrowsValidationException()
        {
            // Arrange
            var eventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
            };

            var mappedEventSeats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatStateModel.Ordered },
            };

            var id = 99;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(EventArea));
            _eventSeatRepositoryMock.Setup(x => x.GetAll()).Returns(eventSeats.AsQueryable());

            for (var i = 0; i < eventSeats.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventSeatModel>(eventSeats[i])).Returns(mappedEventSeats[i]);
            }

            // Act
            var gettingById = _eventSeatService.Invoking(s => s.GetByEventAreaId(id));

            // Assert
            gettingById
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task GetById_EventSeatExists_ReturnsEventSeat()
        {
            // Arrange
            var id = 1;

            var eventSeat = new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available };
            var mappedEventSeat = new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available };

            _mapperMock.Setup(m => m.Map<EventSeatModel>(eventSeat)).Returns(mappedEventSeat);

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventSeat);

            // Act
            var actualEventSeat = await _eventSeatService.GetByIdAsync(id);

            // Assert
            actualEventSeat.Should().BeEquivalentTo(eventSeat);
        }

        [Test]
        public async Task GetById_EventSeatNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(EventSeat));

            // Act
            var gettingById = _eventSeatService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
