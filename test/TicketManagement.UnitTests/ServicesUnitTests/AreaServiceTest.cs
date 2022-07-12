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
    internal class AreaServiceTest
    {
        private Mock<IRepository<Area>> _areaRepositoryMock;
        private IAreaService _areaService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _areaRepositoryMock = new Mock<IRepository<Area>>();
            _mapperMock = new Mock<IMapper>();

            AreaValidator areaValidator = new AreaValidator(_areaRepositoryMock.Object);
            _areaService = new AreaService(_areaRepositoryMock.Object, areaValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidArea_CreatesArea()
        {
            // Arrange
            AreaModel areaToCreate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };
            Area mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };

            _mapperMock.Setup(m => m.Map<Area>(areaToCreate)).Returns(mappedArea);

            // Act
            await _areaService.CreateAsync(areaToCreate);

            // Assert
            _areaRepositoryMock.Verify(x => x.CreateAsync(mappedArea), Times.Once);
        }

        [Test]
        public async Task Create_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            List<Area> areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            AreaModel areaToCreate = new AreaModel { Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };
            Area mappedArea = new Area { Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToCreate)).Returns(mappedArea);

            // Act
            System.Func<Task<int>> creatingArea = _areaService.Invoking(s => s.CreateAsync(areaToCreate));

            // Assert
            await creatingArea.Should().ThrowAsync<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public async Task Create_NullArea_ThrowsValidationException()
        {
            // Arrange
            AreaModel nullArea = null;

            // Act
            System.Func<Task<int>> creatingArea = _areaService.Invoking(s => s.CreateAsync(nullArea));

            // Assert
            await creatingArea.Should().ThrowAsync<ValidationException>()
                .WithMessage("Area is null.");
        }

        [Test]
        public async Task Delete_AreaExists_DeletesArea()
        {
            // Arrange
            Area area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);

            // Act
            await _areaService.DeleteAsync(id);

            // Assert
            _areaRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_AreaNotFound_ThrowsValidationException()
        {
            // Arrange
            List<Area> areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            int notExistingId = 99;

            // Act
            System.Func<Task> deletingArea = _areaService.Invoking(s => s.DeleteAsync(notExistingId));

            // Assert
            await deletingArea.Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task UpdateDescription_ValidArea_UpdatesArea()
        {
            // Arrange
            int id = 1;

            Area area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            List<Area> areas = new List<Area> { area };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            AreaModel areaToUpdate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            Area mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            // Act
            await _areaService.UpdateAsync(areaToUpdate);

            // Assert
            _areaRepositoryMock.Verify(x => x.UpdateAsync(mappedArea), Times.Once);
        }

        [Test]
        public async Task UpdateCoordinates_ValidArea_UpdatesArea()
        {
            // Arrange
            int id = 1;

            Area area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            List<Area> areas = new List<Area> { area };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            AreaModel areaToUpdate = new AreaModel { Id = 1, Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            Area mappedArea = new Area { Id = 1, Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            // Act
            await _areaService.UpdateAsync(areaToUpdate);

            // Assert
            _areaRepositoryMock.Verify(x => x.UpdateAsync(mappedArea), Times.Once);
        }

        [Test]
        public async Task Update_AreaNotFound_ThrowsValidationException()
        {
            // Arrange
            int notExistingId = 1;

            AreaModel areaToUpdate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            Area mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(Area));

            // Act
            System.Func<Task> updatingArea = _areaService.Invoking(s => s.UpdateAsync(areaToUpdate));

            // Assert
            await updatingArea
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            List<Area> areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
            };

            int id = 2;

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(areas.First(a => a.Id == id));

            AreaModel areaToUpdate = new AreaModel { Id = 2, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            Area mappedArea = new Area { Id = 2, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            // Act
            System.Func<Task> updatingArea = _areaService.Invoking(s => s.UpdateAsync(areaToUpdate));

            // Assert
            await updatingArea
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public async Task Update_NullArea_ThrowsValidationException()
        {
            // Arrange
            AreaModel nullArea = null;

            // Act
            System.Func<Task> updatingArea = _areaService.Invoking(s => s.UpdateAsync(nullArea));

            // Assert
            await updatingArea.Should().ThrowAsync<ValidationException>()
                .WithMessage("Area is null.");
        }

        [Test]
        public void GetAll_AreaListNotEmpty_ReturnsAreaList()
        {
            // Arrange
            List<Area> areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new Area { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new Area { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            List<AreaModel> mappedAreas = new List<AreaModel>
            {
                new AreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
                new AreaModel { Id = 2, Description = "Area 2", CoordX = 1, CoordY = 2, LayoutId = 1 },
                new AreaModel { Id = 3, Description = "Area 3", CoordX = 1, CoordY = 1, LayoutId = 2 },
            };

            for (int i = 0; i < areas.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<AreaModel>(areas[i])).Returns(mappedAreas[i]);
            }

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            // Act
            IEnumerable<AreaModel> actualAreas = _areaService.GetAll();

            // Assert
            actualAreas.Should().BeEquivalentTo(mappedAreas);
        }

        [Test]
        public async Task GetById_AreaExists_ReturnsArea()
        {
            // Arrange
            int id = 1;

            Area area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            AreaModel mappedArea = new AreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);

            _mapperMock.Setup(m => m.Map<AreaModel>(area)).Returns(mappedArea);

            // Act
            AreaModel actualArea = await _areaService.GetByIdAsync(id);

            // Assert
            actualArea.Should().BeEquivalentTo(mappedArea);
        }

        [Test]
        public async Task GetById_AreaNotFound_ReturnsNull()
        {
            // Arrange
            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Area));

            // Act
            System.Func<Task<AreaModel>> gettingArea = _areaService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingArea.Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
