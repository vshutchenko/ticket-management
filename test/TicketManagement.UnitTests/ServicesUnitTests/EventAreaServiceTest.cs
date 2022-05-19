using System;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    internal class EventAreaServiceTest
    {
        private Mock<IRepository<EventArea>> _eventAreaRepositoryMock;
        private IEventAreaService _eventAreaService;

        [SetUp]
        public void SetUp()
        {
            _eventAreaRepositoryMock = new Mock<IRepository<EventArea>>();
            _eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object);
        }

        [TestCase(1, 0.2)]
        [TestCase(int.MaxValue, 10)]
        public void SetPrice_ValidParameters(int id, decimal price)
        {
            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(new EventArea());

            _eventAreaService.SetPrice(id, price);

            _eventAreaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
            _eventAreaRepositoryMock.Verify(x => x.Update(It.IsAny<EventArea>()), Times.Once);
        }

        [TestCase(-1, 0.2)]
        [TestCase(0, 10)]
        [TestCase(1, -0.2)]
        [TestCase(int.MaxValue, -10)]
        [TestCase(-1, -1)]
        public void SetPrice_InvalidParameters(int id, decimal price)
        {
            Assert.Throws<ArgumentException>(() => _eventAreaService.SetPrice(id, price));
        }

        [Test]
        public void GetAll_Test()
        {
            _eventAreaService.GetAll();

            _eventAreaRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            _eventAreaService.GetById(id);

            _eventAreaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            Assert.Throws<ArgumentException>(() => _eventAreaService.GetById(id));
        }
    }
}
