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
    internal class LayoutServiceTest
    {
        private Mock<IRepository<Layout>> _layoutRepositoryMock;
        private Mock<IValidationService<Layout>> _layoutValidationMockAlwaysTrue;
        private Mock<IValidationService<Layout>> _layoutValidationMockAlwaysFalse;

        [SetUp]
        public void SetUp()
        {
            _layoutRepositoryMock = new Mock<IRepository<Layout>>();

            _layoutValidationMockAlwaysTrue = new Mock<IValidationService<Layout>>();
            _layoutValidationMockAlwaysTrue.Setup(x => x.Validate(It.IsAny<Layout>())).Returns(true);

            _layoutValidationMockAlwaysFalse = new Mock<IValidationService<Layout>>();
            _layoutValidationMockAlwaysFalse.Setup(x => x.Validate(It.IsAny<Layout>())).Returns(false);
        }

        [Test]
        public void Create_ValidLayout()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            layoutService.Create(new Layout());

            _layoutValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Layout>()), Times.Once);
            _layoutRepositoryMock.Verify(x => x.Create(It.IsAny<Layout>()), Times.Once);
        }

        [Test]
        public void Create_InvalidLayout()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysFalse.Object);

            layoutService.Create(new Layout());

            _layoutValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Layout>()), Times.Once);
            _layoutRepositoryMock.Verify(x => x.Create(It.IsAny<Layout>()), Times.Never);
        }

        [Test]
        public void Create_NullLayout_ThrowsAgrumentNullException()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => layoutService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            layoutService.Delete(id);

            _layoutRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => layoutService.Delete(id));
            _layoutRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidLayout()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            layoutService.Update(new Layout());

            _layoutValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Layout>()), Times.Once);
            _layoutRepositoryMock.Verify(x => x.Update(It.IsAny<Layout>()), Times.Once);
        }

        [Test]
        public void Update_InvalidLayout()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysFalse.Object);

            layoutService.Update(new Layout());

            _layoutValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Layout>()), Times.Once);
            _layoutRepositoryMock.Verify(x => x.Update(It.IsAny<Layout>()), Times.Never);
        }

        [Test]
        public void Update_NullLayout_ThrowsAgrumentNullException()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => layoutService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            layoutService.GetAll();

            _layoutRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            layoutService.GetById(id);

            _layoutRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            ILayoutService layoutService = new LayoutService(_layoutRepositoryMock.Object, _layoutValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => layoutService.GetById(id));
        }
    }
}
