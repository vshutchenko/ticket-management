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
    internal class SeatServiceTest
    {
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private ISeatService _seatService;

        [SetUp]
        public void SetUp()
        {
            _seatRepositoryMock = new Mock<IRepository<Seat>>();

            var seatValidator = new SeatValidator(_seatRepositoryMock.Object);

            _seatService = new SeatService(_seatRepositoryMock.Object, seatValidator);
        }

        [Test]
        public void Create_ValidSeat_CreatesSeat()
        {
            var seatToCreate = new Seat { Id = 1, Row = 1, Number = 4, AreaId = 1, };

            _seatService.CreateAsync(seatToCreate);

            _seatRepositoryMock.Verify(x => x.CreateAsync(seatToCreate), Times.Once);
        }

        [Test]
        public void Create_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var seatToCreate = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            _seatService.Invoking(s => s.CreateAsync(seatToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat with same row and number is already exists in the area.");
        }

        [Test]
        public void Create_NullSeat_ThrowsValidationException()
        {
            _seatService.Invoking(s => s.CreateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public void Delete_SeatExists_DeletesSeat()
        {
            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            int id = 1;

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);

            _seatService.DeleteAsync(id);

            _seatRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public void Delete_SeatNotFound_ThrowsValidationException()
        {
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            int notExistingId = 99;

            _seatService.Invoking(s => s.DeleteAsync(notExistingId))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_ValidSeat_UpdatesSeat()
        {
            int id = 1;

            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };
            var seats = new List<Seat> { seat };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var seatToUpdate = new Seat { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            _seatService.UpdateAsync(seatToUpdate);

            _seatRepositoryMock.Verify(x => x.UpdateAsync(seatToUpdate), Times.Once);
        }

        [Test]
        public void Update_SeatNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var seatToUpdate = new Seat { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).Returns<Venue>(null);

            _seatService.Invoking(s => s.UpdateAsync(seatToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 2, Number = 2, AreaId = 2, },
            };

            int id = 2;

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seats.First(s => s.Id == id));

            var seatToUpdate = new Seat { Id = 2, Row = 1, Number = 1, AreaId = 1, };

            _seatService.Invoking(s => s.UpdateAsync(seatToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat with same row and number is already exists in the area.");
        }

        [Test]
        public void Update_NullSeat_ThrowsValidationException()
        {
            _seatService.Invoking(s => s.UpdateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public void GetAll_SeatListNotEmpty_ReturnsSeatList()
        {
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            _seatService.GetAll().Should().BeEquivalentTo(seats);
        }

        [Test]
        public async Task GetById_SeatExists_ReturnsSeat()
        {
            int id = 1;

            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);

            var actualSeat = await _seatService.GetByIdAsync(id);

            actualSeat.Should().BeEquivalentTo(seat);
        }

        [Test]
        public void GetById_SeatNotFound_ThrowsValidationException()
        {
            int id = 1;

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<Venue>(null);

            _seatService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
