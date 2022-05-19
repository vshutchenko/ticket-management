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
    internal class SeatServiceTest
    {
        private Mock<IRepository<Seat>> _seatRepositoryMock;
        private Mock<IValidationService<Seat>> _seatValidationMockAlwaysTrue;
        private Mock<IValidationService<Seat>> _seatValidationMockAlwaysFalse;

        [SetUp]
        public void SetUp()
        {
            _seatRepositoryMock = new Mock<IRepository<Seat>>();

            _seatValidationMockAlwaysTrue = new Mock<IValidationService<Seat>>();
            _seatValidationMockAlwaysTrue.Setup(x => x.Validate(It.IsAny<Seat>())).Returns(true);

            _seatValidationMockAlwaysFalse = new Mock<IValidationService<Seat>>();
            _seatValidationMockAlwaysFalse.Setup(x => x.Validate(It.IsAny<Seat>())).Returns(false);
        }

        [Test]
        public void Create_ValidSeat()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            seatService.Create(new Seat());

            _seatValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Seat>()), Times.Once);
            _seatRepositoryMock.Verify(x => x.Create(It.IsAny<Seat>()), Times.Once);
        }

        [Test]
        public void Create_InvalidSeat()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysFalse.Object);

            seatService.Create(new Seat());

            _seatValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Seat>()), Times.Once);
            _seatRepositoryMock.Verify(x => x.Create(It.IsAny<Seat>()), Times.Never);
        }

        [Test]
        public void Create_NullSeat_ThrowsAgrumentNullException()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => seatService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            seatService.Delete(id);

            _seatRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => seatService.Delete(id));
            _seatRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidSeat()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            seatService.Update(new Seat());

            _seatValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Seat>()), Times.Once);
            _seatRepositoryMock.Verify(x => x.Update(It.IsAny<Seat>()), Times.Once);
        }

        [Test]
        public void Update_InvalidSeat()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysFalse.Object);

            seatService.Update(new Seat());

            _seatValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Seat>()), Times.Once);
            _seatRepositoryMock.Verify(x => x.Update(It.IsAny<Seat>()), Times.Never);
        }

        [Test]
        public void Update_NullSeat_ThrowsAgrumentNullException()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => seatService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            seatService.GetAll();

            _seatRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            seatService.GetById(id);

            _seatRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            ISeatService seatService = new SeatService(_seatRepositoryMock.Object, _seatValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => seatService.GetById(id));
        }
    }
}
