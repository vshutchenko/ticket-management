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
    internal class EventAreaServiceTest
    {
        private Mock<IRepository<EventArea>> _eventAreaRepositoryMock;
        private IEventAreaService _eventAreaService;

        [SetUp]
        public void SetUp()
        {
            _eventAreaRepositoryMock = new Mock<IRepository<EventArea>>();

            var priceValidator = new PriceValidator();

            _eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, priceValidator);
        }

        [Test]
        public void SetPrice_ValidParameters_SetsPrice()
        {
            int id = 1;
            decimal price = 15;

            var eventArea = new EventArea { Id = 1, CoordX = 1, CoordY = 1, Description = "Area", EventId = 1, Price = 0 };

            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(eventArea);

            _eventAreaService.SetPrice(id, price);

            _eventAreaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
            _eventAreaRepositoryMock.Verify(x => x.Update(eventArea), Times.Once);
        }

        [Test]
        public void SetPrice_InvalidPrice_ThrowsValidationException()
        {
            int id = 1;
            decimal price = -15;

            var eventArea = new EventArea { Id = 1, CoordX = 1, CoordY = 1, Description = "Area", EventId = 1, Price = 0 };

            _eventAreaRepositoryMock.Setup(x => x.GetById(id)).Returns(eventArea);

            _eventAreaService.Invoking(s => s.SetPrice(id, price))
                .Should().Throw<ValidationException>()
                .WithMessage("Price is less than zero.");
        }

        [Test]
        public void SetPrice_EventAreaNotFound_ThrowsValidationException()
        {
            int id = 1;
            decimal price = -15;

            _eventAreaRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<EventArea>(null);

            _eventAreaService.Invoking(s => s.SetPrice(id, price))
                .Should().Throw<ValidationException>()
                .WithMessage("Event area does not exist.");
        }

        [Test]
        public void GetAll_EventAreaListNotEmpty_ReturnsEventAreaList()
        {
            var eventAreas = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventArea { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventArea { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas);

            _eventAreaService.GetAll().Should().BeEquivalentTo(eventAreas);
        }

        [Test]
        public void GetById_EventAreaExists_ReturnsEventArea()
        {
            var eventArea = new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 };

            _eventAreaRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(eventArea);

            _eventAreaService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(eventArea);

            _eventAreaRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GetById_EventAreaNotFound_ReturnsNull()
        {
            _eventAreaRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<EventArea>(null);

            _eventAreaService.GetById(It.IsAny<int>()).Should().BeNull();
        }
    }
}
