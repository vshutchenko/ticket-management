using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class PurchaseServiceTest
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IRepository<Purchase>> _purchaseRepositoryMock;
        private Mock<IRepository<PurchasedSeat>> _purchasedSeatRepositoryMock;
        private Mock<IEventSeatService> _eventSeatServiceMock;
        private Mock<IEventAreaService> _eventAreaServiceMock;
        private IPurchaseService _purchaseService;

        [SetUp]
        public void SetUp()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _purchaseRepositoryMock = new Mock<IRepository<Purchase>>();
            _purchasedSeatRepositoryMock = new Mock<IRepository<PurchasedSeat>>();
            _eventSeatServiceMock = new Mock<IEventSeatService>();
            _eventAreaServiceMock = new Mock<IEventAreaService>();

            _purchaseService = new PurchaseService(
                _userManagerMock.Object,
                _purchaseRepositoryMock.Object,
                _purchasedSeatRepositoryMock.Object,
                _eventSeatServiceMock.Object,
                _eventAreaServiceMock.Object);
        }

        [Test]
        public async Task PurchaseSeatAsync_ValidParameters_PurchasesSeatAsync()
        {
            // Arrange
            var purchase = new PurchaseModel { EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", SeatIds = new List<int> { 1, 2, 3 } };

            var seats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Available },
            };

            var areaId = 1;
            var area = new EventAreaModel { Id = 1, CoordX = 1, CoordY = 2, EventId = 1, Description = "Description", Price = 15 };

            var user = new User { Email = "user1@gmail.com", Balance = 100 };

            for (var i = 0; i < seats.Count; i++)
            {
                _eventSeatServiceMock.Setup(x => x.GetByIdAsync(seats[i].Id)).ReturnsAsync(seats[i]);
            }

            _eventAreaServiceMock.Setup(x => x.GetByIdAsync(areaId)).ReturnsAsync(area);
            _userManagerMock.Setup(x => x.FindByIdAsync(purchase.UserId)).ReturnsAsync(user);

            var purchaseId = 1;
            _purchaseRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Purchase>())).ReturnsAsync(purchaseId);

            // Act
            await _purchaseService.PurchaseSeatAsync(purchase);

            // Assert
            _eventSeatServiceMock.Verify(x => x.SetSeatStateAsync(It.IsAny<int>(), EventSeatStateModel.Ordered), Times.Exactly(purchase.SeatIds.Count));
            _purchasedSeatRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<PurchasedSeat>()), Times.Exactly(purchase.SeatIds.Count));
        }

        [Test]
        public async Task PurchaseSeatAsync_UnsufficientFunds_ThrowsValidationException()
        {
            // Arrange
            var purchase = new PurchaseModel { EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", SeatIds = new List<int> { 1, 2, 3 } };

            var seats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Available },
            };

            var areaId = 1;
            var area = new EventAreaModel { Id = 1, CoordX = 1, CoordY = 2, EventId = 1, Description = "Description", Price = 15 };

            var user = new User { Email = "user1@gmail.com", Balance = 0 };

            for (var i = 0; i < seats.Count; i++)
            {
                _eventSeatServiceMock.Setup(x => x.GetByIdAsync(seats[i].Id)).ReturnsAsync(seats[i]);
            }

            _eventAreaServiceMock.Setup(x => x.GetByIdAsync(areaId)).ReturnsAsync(area);
            _userManagerMock.Setup(x => x.FindByIdAsync(purchase.UserId)).ReturnsAsync(user);

            var purchaseId = 1;
            _purchaseRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Purchase>())).ReturnsAsync(purchaseId);

            // Act
            var purchasing = _purchaseService.Invoking(s => s.PurchaseSeatAsync(purchase));

            // Assert
            await purchasing
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Your account has insufficient funds.");
        }

        [Test]
        public async Task PurchaseSeatAsync_NoSeatsChosen_ThrowsValidationException()
        {
            // Arrange
            var purchase = new PurchaseModel { EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", SeatIds = new List<int>() };

            var seats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Available },
            };

            var areaId = 1;
            var area = new EventAreaModel { Id = 1, CoordX = 1, CoordY = 2, EventId = 1, Description = "Description", Price = 15 };

            var user = new User { Email = "user1@gmail.com", Balance = 100 };

            for (var i = 0; i < seats.Count; i++)
            {
                _eventSeatServiceMock.Setup(x => x.GetByIdAsync(seats[i].Id)).ReturnsAsync(seats[i]);
            }

            _eventAreaServiceMock.Setup(x => x.GetByIdAsync(areaId)).ReturnsAsync(area);
            _userManagerMock.Setup(x => x.FindByIdAsync(purchase.UserId)).ReturnsAsync(user);

            var purchaseId = 1;
            _purchaseRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Purchase>())).ReturnsAsync(purchaseId);

            // Act
            var purchasing = _purchaseService.Invoking(s => s.PurchaseSeatAsync(purchase));

            // Assert
            await purchasing
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("No seats chosen.");
        }

        [Test]
        public async Task PurchaseSeatAsync_OneSeatAlreadyOrdered_ThrowsValidationException()
        {
            // Arrange
            var purchase = new PurchaseModel { EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", SeatIds = new List<int> { 1, 2, 3 } };

            var seats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Available },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Ordered },
            };

            for (var i = 0; i < seats.Count; i++)
            {
                _eventSeatServiceMock.Setup(x => x.GetByIdAsync(seats[i].Id)).ReturnsAsync(seats[i]);
            }

            // Act
            var purchasing = _purchaseService.Invoking(s => s.PurchaseSeatAsync(purchase));

            // Assert
            await purchasing
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("One or more seats have already been ordered.");
        }

        [Test]
        public void GetByUserId_UserExists_ReturnsPurchases()
        {
            // Arrange
            var userId = "13fd42af-6a64-4022-bed4-8c7507cb67b9";

            var purchases = new List<Purchase>
            {
                new Purchase { Id = 1, EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Price = 120 },
            };

            var expectedPurchases = new List<PurchaseModel>
            {
                new PurchaseModel { Id = 1, EventId = 1, UserId = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Price = 120, SeatIds = new List<int> { 1, 2, 3 } },
            };

            var seats = new List<PurchasedSeat>
            {
                new PurchasedSeat { Id = 1, EventSeatId = 1, PurchaseId = 1 },
                new PurchasedSeat { Id = 2, EventSeatId = 2, PurchaseId = 1 },
                new PurchasedSeat { Id = 3, EventSeatId = 3, PurchaseId = 1 },
            };

            _purchaseRepositoryMock.Setup(x => x.GetAll()).Returns(purchases.AsQueryable());

            _purchasedSeatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            // Act
            var actualUser = _purchaseService.GetByUserId(userId);

            // Assert
            actualUser.Should().BeEquivalentTo(expectedPurchases);
        }

        [Test]
        public void GetByPurchaseId_PurchaseExists_ReturnsPurchasedSeats()
        {
            // Arrange
            var purchaseId = 1;

            var purchasedSeats = new List<PurchasedSeat>
            {
                new PurchasedSeat { Id = 1, EventSeatId = 1, PurchaseId = 1 },
                new PurchasedSeat { Id = 2, EventSeatId = 2, PurchaseId = 1 },
                new PurchasedSeat { Id = 3, EventSeatId = 3, PurchaseId = 1 },
            };

            var seats = new List<EventSeatModel>
            {
                new EventSeatModel { Id = 1, EventAreaId = 1, Row = 1, Number = 1, State = EventSeatStateModel.Ordered },
                new EventSeatModel { Id = 2, EventAreaId = 1, Row = 1, Number = 2, State = EventSeatStateModel.Ordered },
                new EventSeatModel { Id = 3, EventAreaId = 1, Row = 2, Number = 1, State = EventSeatStateModel.Ordered },
            };

            _purchasedSeatRepositoryMock.Setup(x => x.GetAll()).Returns(purchasedSeats.AsQueryable());
            _eventSeatServiceMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            // Act
            var actualSeaats = _purchaseService.GetByPurchaseId(purchaseId);

            // Assert
            actualSeaats.Should().BeEquivalentTo(seats);
        }
    }
}
