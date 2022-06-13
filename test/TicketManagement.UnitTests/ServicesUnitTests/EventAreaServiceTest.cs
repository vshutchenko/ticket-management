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

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventArea);

            _eventAreaService.SetPriceAsync(id, price);

            _eventAreaRepositoryMock.Verify(x => x.UpdateAsync(eventArea), Times.Once);
        }

        [Test]
        public void SetPrice_InvalidPrice_ThrowsValidationException()
        {
            int id = 1;
            decimal price = -15;

            var eventArea = new EventArea { Id = 1, CoordX = 1, CoordY = 1, Description = "Area", EventId = 1, Price = 0 };

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventArea);

            _eventAreaService.Invoking(s => s.SetPriceAsync(id, price))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Price is less than zero.");
        }

        [Test]
        public void SetPrice_EventAreaNotFound_ThrowsValidationException()
        {
            int id = 1;
            decimal price = 15;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<EventArea>(null);

            _eventAreaService.Invoking(s => s.SetPriceAsync(id, price))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
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

            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas.AsQueryable());

            _eventAreaService.GetAll().Should().BeEquivalentTo(eventAreas);
        }

        [Test]
        public async Task GetById_EventAreaExists_ReturnsEventArea()
        {
            int id = 1;

            var eventArea = new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 };

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventArea);

            var actualEventArea = await _eventAreaService.GetByIdAsync(id);

            actualEventArea.Should().BeEquivalentTo(eventArea);
        }

        [Test]
        public void GetById_EventAreaNotFound_ThrowsValidationException()
        {
            int id = 1;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<EventArea>(null);

            _eventAreaService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
