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

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventSeat);

            _eventSeatService.SetSeatStateAsync(id, EventSeatState.Ordered);

            _eventSeatRepositoryMock.Verify(x => x.UpdateAsync(eventSeat), Times.Once);
        }

        [Test]
        public void SetSeatState_SeatNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).Returns<EventSeat>(null);

            _eventSeatService.Invoking(s => s.SetSeatStateAsync(notExistingId, EventSeatState.Ordered))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
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

            _eventSeatRepositoryMock.Setup(x => x.GetAll()).Returns(eventSeats.AsQueryable());

            _eventSeatService.GetAll().Should().BeEquivalentTo(eventSeats);
        }

        [Test]
        public async Task GetById_EventSeatExists_ReturnsEventSeat()
        {
            int id = 1;

            var eventSeat = new EventSeat { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatState.Available };

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventSeat);

            var actualEventSeat = await _eventSeatService.GetByIdAsync(id);

            actualEventSeat.Should().BeEquivalentTo(eventSeat);
        }

        [Test]
        public void GetById_EventSeatNotFound_ThrowsValidationException()
        {
            int id = 1;

            _eventSeatRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<Layout>(null);

            _eventSeatService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
