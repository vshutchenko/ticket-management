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

            var areaValidator = new AreaValidator(_areaRepositoryMock.Object);
            _areaService = new AreaService(_areaRepositoryMock.Object, areaValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidArea_CreatesArea()
        {
            var areaToCreate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };
            var mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 3 };

            _mapperMock.Setup(m => m.Map<Area>(areaToCreate)).Returns(mappedArea);

            await _areaService.CreateAsync(areaToCreate);

            _areaRepositoryMock.Verify(x => x.CreateAsync(mappedArea), Times.Once);
        }

        [Test]
        public async Task Create_AreaWithSameDescriptionExists_ThrowsValidationException()
        {
            var areas = new List<Area>
            {
                new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 },
            };

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var areaToCreate = new AreaModel { Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };
            var mappedArea = new Area { Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToCreate)).Returns(mappedArea);

            await _areaService.Invoking(s => s.CreateAsync(areaToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public void Create_NullArea_ThrowsValidationException()
        {
            _areaService.Invoking(s => s.CreateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Area is null.");
        }

        [Test]
        public void Delete_AreaExists_DeletesArea()
        {
            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);

            _areaService.DeleteAsync(id);

            _areaRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
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

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            int notExistingId = 99;

            _areaService.Invoking(s => s.DeleteAsync(notExistingId))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void UpdateDescription_ValidArea_UpdatesArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            var areas = new List<Area> { area };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var areaToUpdate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            var mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            _areaService.UpdateAsync(areaToUpdate);

            _areaRepositoryMock.Verify(x => x.UpdateAsync(mappedArea), Times.Once);
        }

        [Test]
        public void UpdateCoordinates_ValidArea_UpdatesArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            var areas = new List<Area> { area };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);
            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            var areaToUpdate = new AreaModel { Id = 1, Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            var mappedArea = new Area { Id = 1, Description = "Area 1", CoordX = 2, CoordY = 2, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            _areaService.UpdateAsync(areaToUpdate);

            _areaRepositoryMock.Verify(x => x.UpdateAsync(mappedArea), Times.Once);
        }

        [Test]
        public void Update_AreaNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var areaToUpdate = new AreaModel { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            var mappedArea = new Area { Id = 1, Description = "New Area", CoordX = 1, CoordY = 1, LayoutId = 4 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).Returns<Venue>(null);

            _areaService.Invoking(s => s.UpdateAsync(areaToUpdate))
                .Should().ThrowAsync<ValidationException>()
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

            _areaRepositoryMock.Setup(x => x.GetAll()).Returns(areas.AsQueryable());

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(areas.First(a => a.Id == id));

            var areaToUpdate = new AreaModel { Id = 2, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            var mappedArea = new Area { Id = 2, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _mapperMock.Setup(m => m.Map<Area>(areaToUpdate)).Returns(mappedArea);

            _areaService.Invoking(s => s.UpdateAsync(areaToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Area description should be unique in the layout.");
        }

        [Test]
        public void Update_NullArea_ThrowsValidationException()
        {
            _areaService.Invoking(s => s.UpdateAsync(null))
                .Should().ThrowAsync<ValidationException>()
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

            var mappedAreas = new List<AreaModel>
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

            _areaService.GetAll().Should().BeEquivalentTo(mappedAreas);
        }

        [Test]
        public async Task GetById_AreaExists_ReturnsArea()
        {
            int id = 1;

            var area = new Area { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };
            var mappedArea = new AreaModel { Id = 1, Description = "Area 1", CoordX = 1, CoordY = 1, LayoutId = 1 };

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(area);

            _mapperMock.Setup(m => m.Map<AreaModel>(area)).Returns(mappedArea);

            var actualArea = await _areaService.GetByIdAsync(id);
            actualArea.Should().BeEquivalentTo(mappedArea);
        }

        [Test]
        public void GetById_AreaNotFound_ReturnsNull()
        {
            int id = 1;

            _areaRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<Area>(null);

            _areaService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
