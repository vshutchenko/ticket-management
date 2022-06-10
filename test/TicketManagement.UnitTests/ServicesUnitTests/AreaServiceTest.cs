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

        [SetUp]
        public void SetUp()
        {
            _areaRepositoryMock = new Mock<IRepository<Area>>();

            var areaValidator = new AreaValidator(_areaRepositoryMock.Object);
            _areaService = new AreaService(_areaRepositoryMock.Object, areaValidator);
        }

        [Test]
        public void Create_ValidArea_CreatesArea()
        {
            var areaToCreate = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };

            _areaService.Create(areaToCreate);

            _areaRepositoryMock.Verify(x => x.Create(areaToCreate), Times.Once);
        }

        [Test]
        public void Create_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas);

            var areaToCreate = new Area { Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

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
            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns(area);

            _areaService.Delete(id);

            _areaRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [Test]
        public void Delete_AreaNotFound_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas);

            int notExistingId = 99;

            _areaService.Invoking(s => s.Delete(notExistingId))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void UpdateDescription_ValidArea_UpdatesArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Area> { area });

            var areaToUpdate = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _areaService.Update(areaToUpdate);

            _areaRepositoryMock.Verify(x => x.Update(areaToUpdate), Times.Once);
        }

        [Test]
        public void UpdateCoordinates_ValidArea_UpdatesArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Area> { area });

            var areaToUpdate = new Area { Id = 1, Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            _areaService.Update(areaToUpdate);

            _areaRepositoryMock.Verify(x => x.Update(areaToUpdate), Times.Once);
        }

        [Test]
        public void Update_AreaNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var areaToUpdate = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _areaRepositoryMock.Setup(x => x.GetById(notExistingId)).Returns<Venue>(null);

            _areaService.Invoking(s => s.Update(areaToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
            };

            int id = 2;

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas);

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns(areas.First(a => a.Id == id));

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
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas);

            _areaService.GetAll().Should().BeEquivalentTo(areas);
        }

        [Test]
        public void GetById_AreaExists_ReturnsArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns(area);

            _areaService.GetById(id).Should().BeEquivalentTo(area);
        }

        [Test]
        public void GetById_AreaNotFound_ReturnsNull()
        {
            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetById(id)).Returns<Area>(null);

            _areaService.Invoking(s => s.GetById(id))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
