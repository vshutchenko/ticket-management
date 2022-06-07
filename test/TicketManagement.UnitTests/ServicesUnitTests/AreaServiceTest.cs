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
    internal class AreaServiceTest
    {
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private IAreaService _areaService;
        private List<Area> _areas;

        [SetUp]
        public void SetUp()
        {
            _areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            _areaRepositoryMock = new Mock<IRepository<Area>>();
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(_areas);

            var areaValidator = new AreaValidator(_areaRepositoryMock.Object);
            _areaService = new AreaService(_areaRepositoryMock.Object, areaValidator);
        }

        [Test]
        public void Create_ValidArea_CreatesArea()
        {
            var areaToCreate = new Area { Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };

            _areaService.Create(areaToCreate);

            _areaRepositoryMock.Verify(x => x.Create(areaToCreate), Times.Once);
        }

        [Test]
        public void Create_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            var areaToCreate = new Area { Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaService.Invoking(s => s.Create(areaToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public void Create_NullArea_ThrowsValidationException()
        {
            _areaService.Invoking(s => s.Create(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Area is null.");
        }

        [Test]
        public void Delete_AreaExists_DeletesArea()
        {
            _areaService.Delete(It.IsAny<int>());

            _areaRepositoryMock.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Update_ValidArea_UpdatesArea()
        {
            var areaToUpdate = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _areaService.Update(areaToUpdate);

            _areaRepositoryMock.Verify(x => x.Update(areaToUpdate), Times.Once);
        }

        [Test]
        public void Update_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            var areaToUpdate = new Area { Id = 2, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaService.Invoking(s => s.Update(areaToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public void Update_NullArea_ThrowsValidationException()
        {
            _areaService.Invoking(s => s.Update(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Area is null.");
        }

        [Test]
        public void GetAll_AreaListNotEmpty_ReturnsAreaList()
        {
            var areas = _areaService.GetAll().ToList();

            areas.Should().BeEquivalentTo(_areas);
        }

        [Test]
        public void GetById_AreaExists_ReturnsArea()
        {
            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(area);

            _areaService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(area);
        }

        [Test]
        public void GetById_AreaNotFound_ReturnsNull()
        {
            _areaRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Area>(null);

            _areaService.GetById(It.IsAny<int>()).Should().BeNull();
        }
    }
}
