using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
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
    internal class SeatServiceTest
    {
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private ISeatService _seatService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _seatRepositoryMock = new Mock<IRepository<Seat>>();
            _mapperMock = new Mock<IMapper>();

            SeatValidator seatValidator = new SeatValidator(_seatRepositoryMock.Object);

            _seatService = new SeatService(_seatRepositoryMock.Object, seatValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidSeat_CreatesSeat()
        {
            // Arrange
            SeatModel seatToCreate = new SeatModel { Id = 1, Row = 1, Number = 4, AreaId = 1, };

            Seat mappedSeat = new Seat { Id = 1, Row = 1, Number = 4, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToCreate)).Returns(mappedSeat);

            // Act
            await _seatService.CreateAsync(seatToCreate);

            // Assert
            _seatRepositoryMock.Verify(x => x.CreateAsync(mappedSeat), Times.Once);
        }

        [Test]
        public async Task Create_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            // Arrange
            List<Seat> seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            SeatModel seatToCreate = new SeatModel { Row = 1, Number = 1, AreaId = 1, };

            Seat mappedSeat = new Seat { Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToCreate)).Returns(mappedSeat);

            // Act
            System.Func<Task<int>> creatingSeat = _seatService.Invoking(s => s.CreateAsync(seatToCreate));

            // Assert
            await creatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat with same row and number is already exists in the area.");
        }

        [Test]
        public async Task Create_NullSeat_ThrowsValidationException()
        {
            // Arrange
            SeatModel nullSeat = null;

            // Act
            System.Func<Task<int>> updatingSeat = _seatService.Invoking(s => s.CreateAsync(nullSeat));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public async Task Delete_SeatExists_DeletesSeat()
        {
            // Arrange
            Seat seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            int id = 1;

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);

            // Act
            await _seatService.DeleteAsync(id);

            // Assert
            _seatRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_SeatNotFound_ThrowsValidationException()
        {
            // Arrange
            List<Seat> seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            int notExistingId = 99;

            // Act
            System.Func<Task> deletingSeat = _seatService.Invoking(s => s.DeleteAsync(notExistingId));

            // Assert
            await deletingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_ValidSeat_UpdatesSeat()
        {
            // Arrange
            int id = 1;

            Seat seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };
            List<Seat> seats = new List<Seat> { seat };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            SeatModel seatToUpdate = new SeatModel { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            Seat mappedSeat = new Seat { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToUpdate)).Returns(mappedSeat);

            // Act
            await _seatService.UpdateAsync(seatToUpdate);

            // Assert
            _seatRepositoryMock.Verify(x => x.UpdateAsync(mappedSeat), Times.Once);
        }

        [Test]
        public async Task Update_SeatNotFound_ThrowsValidationException()
        {
            // Arrange
            int notExistingId = 1;

            SeatModel seatToUpdate = new SeatModel { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(Seat));

            // Act
            System.Func<Task> updatingSeat = _seatService.Invoking(s => s.UpdateAsync(seatToUpdate));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            // Arrange
            List<Seat> seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 2, Number = 2, AreaId = 2, },
            };

            int id = 2;

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seats.First(s => s.Id == id));

            SeatModel seatToUpdate = new SeatModel { Id = 2, Row = 1, Number = 1, AreaId = 1, };

            Seat mappedSeat = new Seat { Id = 2, Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToUpdate)).Returns(mappedSeat);

            // Act
            System.Func<Task> updatingSeat = _seatService.Invoking(s => s.UpdateAsync(seatToUpdate));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat with same row and number is already exists in the area.");
        }

        [Test]
        public async Task Update_NullSeat_ThrowsValidationException()
        {
            // Arrange
            SeatModel nullSeat = null;

            // Act
            System.Func<Task> updatingSeat = _seatService.Invoking(s => s.UpdateAsync(nullSeat));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public void GetAll_SeatListNotEmpty_ReturnsSeatList()
        {
            // Arrange
            List<Seat> seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            List<SeatModel> mappedSeats = new List<SeatModel>
            {
                new SeatModel { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new SeatModel { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new SeatModel { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            for (int i = 0; i < seats.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<SeatModel>(seats[i])).Returns(mappedSeats[i]);
            }

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            // Act
            IEnumerable<SeatModel> actualSeats = _seatService.GetAll();

            // Assert
            actualSeats.Should().BeEquivalentTo(seats);
        }

        [Test]
        public async Task GetById_SeatExists_ReturnsSeat()
        {
            // Arrange
            int id = 1;

            Seat seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };
            SeatModel mappedSeat = new SeatModel { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<SeatModel>(seat)).Returns(mappedSeat);

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);

            // Act
            SeatModel actualSeat = await _seatService.GetByIdAsync(id);

            // Assert
            actualSeat.Should().BeEquivalentTo(seat);
        }

        [Test]
        public async Task GetById_SeatNotFound_ThrowsValidationException()
        {
            // Arrange
            int id = 1;

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Seat));

            // Act
            System.Func<Task<SeatModel>> gettingById = _seatService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}