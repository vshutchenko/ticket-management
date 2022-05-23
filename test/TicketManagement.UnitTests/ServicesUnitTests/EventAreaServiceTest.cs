using System;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    internal class EventAreaServiceTest
    {
        private Mock<IRepository<EventArea>> _eventAreaRepositoryMock;
        private Mock<IValidator<decimal>> _priceValidatorMock;
        private Mock<IValidator<decimal>> _priceValidatorThrowsException;

        [SetUp]
        public void SetUp()
        {
            _eventAreaRepositoryMock = new Mock<IRepository<EventArea>>();

            _priceValidatorMock = new Mock<IValidator<decimal>>();

            _priceValidatorThrowsException = new Mock<IValidator<decimal>>();
            _priceValidatorThrowsException.Setup(x => x.Validate(It.IsAny<decimal>())).Throws<ValidationException>();
        }

        [TestCase(1, 0.2)]
        [TestCase(int.MaxValue, 10)]
        public void SetPrice_ValidParameters(int id, decimal price)
        {
            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(new EventArea());

            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorMock.Object);

            eventAreaService.SetPrice(id, price);

            _eventAreaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
            _eventAreaRepositoryMock.Verify(x => x.Update(It.IsAny<EventArea>()), Times.Once);
        }

        [TestCase(1, -0.2)]
        [TestCase(int.MaxValue, -10)]
        public void SetPrice_InvalidPrice(int id, decimal price)
        {
            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(new EventArea());

            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorThrowsException.Object);

            Assert.Throws<ValidationException>(() => eventAreaService.SetPrice(id, price));
        }

        [TestCase(-1, 0.2)]
        [TestCase(0, -10)]
        public void SetPrice_InvalidId(int id, decimal price)
        {
            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(new EventArea());

            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => eventAreaService.SetPrice(id, price));
        }

        [Test]
        public void GetAll_Test()
        {
            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorMock.Object);

            eventAreaService.GetAll();

            _eventAreaRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorMock.Object);

            eventAreaService.GetById(id);

            _eventAreaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            IEventAreaService eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _priceValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => eventAreaService.GetById(id));
        }
    }
}
