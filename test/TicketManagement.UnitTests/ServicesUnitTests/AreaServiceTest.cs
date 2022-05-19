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
    internal class AreaServiceTest
    {
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private Mock<IValidationService<Area>> _areaValidationMockAlwaysTrue;
        private Mock<IValidationService<Area>> _areaValidationMockAlwaysFalse;

        [SetUp]
        public void SetUp()
        {
            _areaRepositoryMock = new Mock<IRepository<Area>>();

            _areaValidationMockAlwaysTrue = new Mock<IValidationService<Area>>();
            _areaValidationMockAlwaysTrue.Setup(x => x.Validate(It.IsAny<Area>())).Returns(true);

            _areaValidationMockAlwaysFalse = new Mock<IValidationService<Area>>();
            _areaValidationMockAlwaysFalse.Setup(x => x.Validate(It.IsAny<Area>())).Returns(false);
        }

        [Test]
        public void Create_ValidArea()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            areaService.Create(new Area());

            _areaValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Area>()), Times.Once);
            _areaRepositoryMock.Verify(x => x.Create(It.IsAny<Area>()), Times.Once);
        }

        [Test]
        public void Create_InvalidArea()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysFalse.Object);

            areaService.Create(new Area());

            _areaValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Area>()), Times.Once);
            _areaRepositoryMock.Verify(x => x.Create(It.IsAny<Area>()), Times.Never);
        }

        [Test]
        public void Create_NullArea_ThrowsAgrumentNullException()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => areaService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            areaService.Delete(id);

            _areaRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => areaService.Delete(id));
            _areaRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidArea()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            areaService.Update(new Area());

            _areaValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Area>()), Times.Once);
            _areaRepositoryMock.Verify(x => x.Update(It.IsAny<Area>()), Times.Once);
        }

        [Test]
        public void Update_InvalidArea()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysFalse.Object);

            areaService.Update(new Area());

            _areaValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Area>()), Times.Once);
            _areaRepositoryMock.Verify(x => x.Update(It.IsAny<Area>()), Times.Never);
        }

        [Test]
        public void Update_NullArea_ThrowsAgrumentNullException()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => areaService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            areaService.GetAll();

            _areaRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            areaService.GetById(id);

            _areaRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            IAreaService areaService = new AreaService(_areaRepositoryMock.Object, _areaValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => areaService.GetById(id));
        }
    }
}
