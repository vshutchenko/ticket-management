using System;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
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

        [TestCase(1, 0)]
        [TestCase(int.MaxValue, 1)]
        public void SetSeatState_ValidParameters(int id, EventSeatState state)
        {
            _eventSeatRepositoryMock.Setup(x => x.GetById(id)).Returns(new EventSeat());

            _eventSeatService.SetSeatState(id, state);

            _eventSeatRepositoryMock.Verify(x => x.GetById(id), Times.Once);
            _eventSeatRepositoryMock.Verify(x => x.Update(It.IsAny<EventSeat>()), Times.Once);
        }

        [TestCase(-1, 0)]
        [TestCase(0, 1)]
        public void SetSeatState_InvalidParameters(int id, EventSeatState state)
        {
            Assert.Throws<ArgumentException>(() => _eventSeatService.SetSeatState(id, state));
        }

        [Test]
        public void GetAll_Test()
        {
            _eventSeatService.GetAll();

            _eventSeatRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            _eventSeatService.GetById(id);

            _eventSeatRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            Assert.Throws<ArgumentException>(() => _eventSeatService.GetById(id));
        }
    }
}
