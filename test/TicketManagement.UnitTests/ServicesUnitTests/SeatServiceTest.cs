using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class SeatServiceTest
    {
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private ISeatService _seatService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _seatRepositoryMock = new Mock<IRepository<Seat>>();
            _areaRepositoryMock = new Mock<IRepository<Area>>();
            _mapperMock = new Mock<IMapper>();

            var seatValidator = new SeatValidator(_seatRepositoryMock.Object);

            _seatService = new SeatService(_seatRepositoryMock.Object, _areaRepositoryMock.Object, seatValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidSeat_CreatesSeat()
        {
            // Arrange
            var seatToCreate = new SeatModel { Id = 1, Row = 1, Number = 4, AreaId = 1, };

            var mappedSeat = new Seat { Id = 1, Row = 1, Number = 4, AreaId = 1, };

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
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var seatToCreate = new SeatModel { Row = 1, Number = 1, AreaId = 1, };

            var mappedSeat = new Seat { Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToCreate)).Returns(mappedSeat);

            // Act
            var creatingSeat = _seatService.Invoking(s => s.CreateAsync(seatToCreate));

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
            var updatingSeat = _seatService.Invoking(s => s.CreateAsync(nullSeat));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public async Task Delete_SeatExists_DeletesSeat()
        {
            // Arrange
            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            var id = 1;

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
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var notExistingId = 99;

            // Act
            var deletingSeat = _seatService.Invoking(s => s.DeleteAsync(notExistingId));

            // Assert
            await deletingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_ValidSeat_UpdatesSeat()
        {
            // Arrange
            var id = 1;

            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };
            var seats = new List<Seat> { seat };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);
            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            var seatToUpdate = new SeatModel { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            var mappedSeat = new Seat { Id = 1, Row = 2, Number = 2, AreaId = 1, };

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
            var notExistingId = 1;

            var seatToUpdate = new SeatModel { Id = 1, Row = 2, Number = 2, AreaId = 1, };

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(Seat));

            // Act
            var updatingSeat = _seatService.Invoking(s => s.UpdateAsync(seatToUpdate));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_SeatWithSameRowAndNumberExists_ThrowsValidationException()
        {
            // Arrange
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 2, Number = 2, AreaId = 2, },
            };

            var id = 2;

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seats.First(s => s.Id == id));

            var seatToUpdate = new SeatModel { Id = 2, Row = 1, Number = 1, AreaId = 1, };

            var mappedSeat = new Seat { Id = 2, Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<Seat>(seatToUpdate)).Returns(mappedSeat);

            // Act
            var updatingSeat = _seatService.Invoking(s => s.UpdateAsync(seatToUpdate));

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
            var updatingSeat = _seatService.Invoking(s => s.UpdateAsync(nullSeat));

            // Assert
            await updatingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Seat is null.");
        }

        [Test]
        public void GetAll_SeatListNotEmpty_ReturnsSeatList()
        {
            // Arrange
            var seats = new List<Seat>
            {
                new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new Seat { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new Seat { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            var mappedSeats = new List<SeatModel>
            {
                new SeatModel { Id = 1, Row = 1, Number = 1, AreaId = 1, },
                new SeatModel { Id = 2, Row = 1, Number = 2, AreaId = 1, },
                new SeatModel { Id = 3, Row = 1, Number = 3, AreaId = 1, },
            };

            for (var i = 0; i < seats.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<SeatModel>(seats[i])).Returns(mappedSeats[i]);
            }

            _seatRepositoryMock.Setup(x => x.GetAll()).Returns(seats.AsQueryable());

            // Act
            var actualSeats = _seatService.GetAll();

            // Assert
            actualSeats.Should().BeEquivalentTo(seats);
        }

        [Test]
        public async Task GetById_SeatExists_ReturnsSeat()
        {
            // Arrange
            var id = 1;

            var seat = new Seat { Id = 1, Row = 1, Number = 1, AreaId = 1, };
            var mappedSeat = new SeatModel { Id = 1, Row = 1, Number = 1, AreaId = 1, };

            _mapperMock.Setup(m => m.Map<SeatModel>(seat)).Returns(mappedSeat);

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(seat);

            // Act
            var actualSeat = await _seatService.GetByIdAsync(id);

            // Assert
            actualSeat.Should().BeEquivalentTo(seat);
        }

        [Test]
        public async Task GetById_SeatNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            _seatRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Seat));

            // Act
            var gettingById = _seatService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}