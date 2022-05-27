using System.Collections.Generic;
using System.Linq;
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
    internal class SeatServiceTest
    {
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private ISeatService _seatService;
        private List<Seat> _seats;

        [SetUp]
        public void SetUp()
        {
            _seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            _seatRepositoryMock = new Mock<IRepository<Seat>>();

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(_seats);

            var seatValidator = new SeatValidator(_seatRepositoryMock.Object);

            _seatService = new SeatService(_seatRepositoryMock.Object, seatValidator);
        }

        [Test]
        public void Create_ValidSeat_SeatCreated()
        {
            var seatToCreate = new Seat { Id = 1, Row = 1, Number = 4, AreaId = 1, };

            _seatService.Create(seatToCreate);

            _seatRepositoryMock.Verify(x => x.Create(seatToCreate), Times.Once);
        }

        [Test]
        public void Create_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            var seatToCreate = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            Assert.Throws<ValidationException>(() => _seatService.Create(seatToCreate));
        }

        [Test]
        public void Create_NullSeat_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _seatService.Create(null));
        }

        [TestCase(1)]
        public void Delete_SeatDeleted(int id)
        {
            _seatService.Delete(id);

            _seatRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [Test]
        public void Update_ValidSeat_SeatUpdated()
        {
            var seatToUpdate = new Seat { Id = 1, Row = 2, Number = 1, AreaId = 1, };

            _seatService.Update(seatToUpdate);

            _seatRepositoryMock.Verify(x => x.Update(seatToUpdate), Times.Once);
        }

        [Test]
        public void Update_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            var seatToUpdate = new Seat { Id = 1, Row = 1, Number = 2, AreaId = 1, };

            Assert.Throws<ValidationException>(() => _seatService.Update(seatToUpdate));
        }

        [Test]
        public void Update_NullSeat_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _seatService.Update(null));
        }

        [Test]
        public void GetAll_SeatListReturned()
        {
            var seats = _seatService.GetAll().ToList();

            seats.Should().BeEquivalentTo(_seats);
        }

        [Test]
        public void GetById_SeatReturned()
        {
            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            _seatRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(seat);

            _seatService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(seat);
        }

        [Test]
        public void GetById_SeatNotFound_NullReturned()
        {
            _seatRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Seat>(null);

            Assert.IsNull(_seatService.GetById(It.IsAny<int>()));
        }
    }
}
