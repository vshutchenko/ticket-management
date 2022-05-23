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
    internal class EventServiceTest
    {
        private Mock<IRepository<Event>> _eventRepositoryMock;
        private Mock<IValidator<Event>> _eventValidatorMock;
        private Mock<IValidator<Event>> _eventValidatorMockThrowsException;

        [SetUp]
        public void SetUp()
        {
            _eventRepositoryMock = new Mock<IRepository<Event>>();

            _eventValidatorMock = new Mock<IValidator<Event>>();

            _eventValidatorMockThrowsException = new Mock<IValidator<Event>>();
            _eventValidatorMockThrowsException.Setup(x => x.Validate(It.IsAny<Event>())).Throws<ValidationException>();
        }

        [Test]
        public void Create_ValidEvent()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            eventService.Create(new Event());

            _eventValidatorMock.Verify(x => x.Validate(It.IsAny<Event>()), Times.Once);
            _eventRepositoryMock.Verify(x => x.Create(It.IsAny<Event>()), Times.Once);
        }

        [Test]
        public void Create_InvalidEvent()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMockThrowsException.Object);

            Assert.Throws<ValidationException>(() => eventService.Create(new Event()));
        }

        [Test]
        public void Create_NullEvent_ThrowsAgrumentNullException()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMockThrowsException.Object);

            Assert.Throws<ArgumentNullException>(() => eventService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            eventService.Delete(id);

            _eventRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => eventService.Delete(id));
            _eventRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidEvent()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            eventService.Update(new Event());

            _eventValidatorMock.Verify(x => x.Validate(It.IsAny<Event>()), Times.Once);
            _eventRepositoryMock.Verify(x => x.Update(It.IsAny<Event>()), Times.Once);
        }

        [Test]
        public void Update_InvalidEvent()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMockThrowsException.Object);

            Assert.Throws<ValidationException>(() => eventService.Update(new Event()));
        }

        [Test]
        public void Update_NullEvent_ThrowsAgrumentNullException()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMockThrowsException.Object);

            Assert.Throws<ArgumentNullException>(() => eventService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            eventService.GetAll();

            _eventRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            eventService.GetById(id);

            _eventRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            IEventService eventService = new EventService(_eventRepositoryMock.Object, _eventValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => eventService.GetById(id));
        }
    }
}
