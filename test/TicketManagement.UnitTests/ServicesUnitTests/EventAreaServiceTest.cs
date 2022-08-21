using System;
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
    internal class EventAreaServiceTest
    {
        private Mock<IRepository<EventArea>> _eventAreaRepositoryMock;
        private Mock<IRepository<Event>> _eventRepositoryMock;
        private IEventAreaService _eventAreaService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _eventAreaRepositoryMock = new Mock<IRepository<EventArea>>();
            _eventRepositoryMock = new Mock<IRepository<Event>>();
            _mapperMock = new Mock<IMapper>();

            var priceValidator = new PriceValidator();

            _eventAreaService = new EventAreaService(_eventAreaRepositoryMock.Object, _eventRepositoryMock.Object, priceValidator, _mapperMock.Object);
        }

        [Test]
        public async Task SetPrice_ValidParameters_SetsPrice()
        {
            // Arrange
            var id = 1;
            decimal price = 15;

            var eventArea = new EventArea { Id = 1, CoordX = 1, CoordY = 1, Description = "Area", EventId = 1, Price = 0 };

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventArea);
            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(new List<EventArea> { eventArea }.AsQueryable());

            // Act
            await _eventAreaService.SetPriceAsync(id, price);

            // Assert
            _eventAreaRepositoryMock.Verify(x => x.UpdateAsync(eventArea), Times.Once);
        }

        [Test]
        public async Task SetPrice_InvalidPrice_ThrowsValidationException()
        {
            // Arrange
            var id = 1;
            decimal price = -15;

            var eventArea = new EventArea { Id = 1, CoordX = 1, CoordY = 1, Description = "Area", EventId = 1, Price = 0 };
            var eventAreaList = new List<EventArea> { eventArea };

            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreaList.AsQueryable());

            // Act
            var settingPrice = _eventAreaService.Invoking(s => s.SetPriceAsync(id, price));

            // Assert
            await settingPrice
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Price is less than zero.");
        }

        [Test]
        public async Task SetPrice_EventAreaNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = 1;
            decimal price = 15;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(EventArea));

            // Act
            var settingPrice = _eventAreaService.Invoking(s => s.SetPriceAsync(id, price));

            // Assert
            await settingPrice
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void GetAll_EventAreaListNotEmpty_ReturnsEventAreaList()
        {
            // Arrange
            var eventAreas = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventArea { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventArea { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            var mappedEventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            for (var i = 0; i < eventAreas.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventAreaModel>(eventAreas[i])).Returns(mappedEventAreas[i]);
            }

            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas.AsQueryable());

            // Act
            var actualAreas = _eventAreaService.GetAll();

            // Assert
            actualAreas.Should().BeEquivalentTo(eventAreas);
        }

        [Test]
        public void GetByEventId_EventSeatListNotEmpty_ReturnsEventSeatList()
        {
            // Arrange
            var eventAreas = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventArea { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventArea { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            var mappedEventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            var id = 1;
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1", Description = "Description 1", LayoutId = 1, StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023, 1, 2) },
            };

            _eventRepositoryMock.Setup(x => x.GetAll()).Returns(events.AsQueryable());
            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas.AsQueryable());

            for (var i = 0; i < eventAreas.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventAreaModel>(eventAreas[i])).Returns(mappedEventAreas[i]);
            }

            // Act
            var actualArea = _eventAreaService.GetByEventId(id);

            // Assert
            actualArea.Should().BeEquivalentTo(mappedEventAreas);
        }

        [Test]
        public void GetByEventId_EventAreaNotFound_ThrowsValidationException()
        {
            // Arrange
            var eventAreas = new List<EventArea>
            {
                new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventArea { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventArea { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            var mappedEventAreas = new List<EventAreaModel>
            {
                new EventAreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, EventId = 1, Price = 1 },
                new EventAreaModel { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 3, EventId = 1, Price = 1 },
            };

            var id = 99;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(EventArea));
            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(eventAreas.AsQueryable());

            for (var i = 0; i < eventAreas.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<EventAreaModel>(eventAreas[i])).Returns(mappedEventAreas[i]);
            }

            // Act
            var gettingById = _eventAreaService.Invoking(s => s.GetByEventId(id));

            // Assert
            gettingById.Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task GetById_EventAreaExists_ReturnsEventArea()
        {
            // Arrange
            var id = 1;

            var eventArea = new EventArea { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 };
            var mappedEventArea = new EventAreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, EventId = 1, Price = 1 };

            _mapperMock.Setup(m => m.Map<EventAreaModel>(eventArea)).Returns(mappedEventArea);

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(eventArea);
            _eventAreaRepositoryMock.Setup(x => x.GetAll()).Returns(new List<EventArea> { eventArea }.AsQueryable());

            // Act
            var actualEventArea = await _eventAreaService.GetByIdAsync(id);

            // Assert
            actualEventArea.Should().BeEquivalentTo(mappedEventArea);
        }

        [Test]
        public async Task GetById_EventAreaNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            _eventAreaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(EventArea));

            // Act
            var gettingById = _eventAreaService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
