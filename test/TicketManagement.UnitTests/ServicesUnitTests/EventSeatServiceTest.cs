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
    internal class EventSeatServiceTest
    {
        private Mock<IRepository<EventSeat>> _eventSeatRepositoryMock;
        private IEventSeatService _eventSeatService;

        [SetUp]
        public void SetUp()
        {
            _eventSeatRepositoryMock = new Mock<IRepository<EventSeat>>();
            _eventSeatService = new EventSeatService(_eventSeatRepositoryMock.Object);
        }

        [Test]
        public void SetSeatState_ValidParameters_SetsState()
        {
            int id = 1;
            var eventSeat = new EventSeat { Id = 1, EventAreaId = 1, Number = 1, Row = 1, State = EventSeatState.Available };

            _eventSeatRepositoryMock.Setup(x => x.GetById(id)).Returns(eventSeat);

            _eventSeatService.SetSeatState(id, EventSeatState.Ordered);

            _eventSeatRepositoryMock.Verify(x => x.Update(eventSeat), Times.Once);
        }

        [Test]
        public void SetSeatState_InvalidId_ThrowsValidationException()
        {
            _eventSeatRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<EventSeat>(null);

            _eventSeatService.Invoking(s => s.SetSeatState(1, EventSeatState.Ordered))
                .Should().Throw<ValidationException>()
                .WithMessage("Event seat does not exist.");
        }

        [Test]
        public void GetAll_EventSeatListNotEmpty_ReturnsEventSeatList()
        {
            var eventSeats = new List<EventSeat>
            {
                new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatState.Available },
                new EventSeat { Id = 2, EventAreaId = 1, Row = 1, Number = 3, State = EventSeatState.Ordered },
            };

            _eventSeatRepositoryMock.Setup(x => x.GetAll()).Returns(eventSeats);

            _eventSeatService.GetAll().Should().BeEquivalentTo(eventSeats);
        }

        [Test]
        public void GetById_EventSeatExists_ReturnsEventSeat()
        {
            var eventSeat = new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available };

            _eventSeatRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(eventSeat);

            _eventSeatService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(eventSeat);
        }

        [Test]
        public void GetById_EventSeatNotFound_ReturnsNull()
        {
            _eventSeatRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Layout>(null);

            _eventSeatService.GetById(It.IsAny<int>()).Should().BeNull();
        }
    }
}
