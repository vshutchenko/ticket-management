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
    internal class LayoutServiceTest
    {
        private Mock<IRepository<Layout>> _layoutRepositoryMock;
        private ILayoutService _layoutService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _layoutRepositoryMock = new Mock<IRepository<Layout>>();
            _mapperMock = new Mock<IMapper>();

            var layoutValidator = new LayoutValidator(_layoutRepositoryMock.Object);

            _layoutService = new LayoutService(_layoutRepositoryMock.Object, layoutValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidLayout_CreatesLayout()
        {
            // Arrange
            var layoutToCreate = new LayoutModel { Id = 1, Description = "New Layout", VenueId = 1, };
            var mappedLayout = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _mapperMock.Setup(m => m.Map<Layout>(layoutToCreate)).Returns(mappedLayout);

            // Act
            await _layoutService.CreateAsync(layoutToCreate);

            // Assert
            _layoutRepositoryMock.Verify(x => x.CreateAsync(mappedLayout), Times.Once);
        }

        [Test]
        public async Task Create_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            var layoutToCreate = new LayoutModel { Description = "Layout 1", VenueId = 1, };
            var mappedLayout = new Layout { Description = "Layout 1", VenueId = 1, };

            _mapperMock.Setup(m => m.Map<Layout>(layoutToCreate)).Returns(mappedLayout);

            // Act
            var creatingLayout = _layoutService.Invoking(s => s.CreateAsync(layoutToCreate));

            // Assert
            await creatingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public async Task Create_NullLayout_ThrowsValidationException()
        {
            // Arrange
            LayoutModel nullLayout = null;

            // Act
            var creatingLayout = _layoutService.Invoking(s => s.CreateAsync(nullLayout));

            // Assert
            await creatingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Layout is null.");
        }

        [Test]
        public async Task Delete_LayoutExists_DeletesLayout()
        {
            // Arrange
            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            var id = 1;

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);

            // Act
            await _layoutService.DeleteAsync(id);

            // Assert
            _layoutRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_LayoutNotFound_ThrowsValidationException()
        {
            // Arrange
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
                new Layout { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            var notExistingId = 99;

            // Act
            var deletingLayout = _layoutService.Invoking(s => s.DeleteAsync(notExistingId));

            // Assert
            await deletingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_ValidLayout_UpdatesLayout()
        {
            // Arrange
            var id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };
            var layouts = new List<Layout> { layout };

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);
            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            var layoutToUpdate = new LayoutModel { Id = 1, Description = "New Layout", VenueId = 1, };
            var mappedLayout = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _mapperMock.Setup(m => m.Map<Layout>(layoutToUpdate)).Returns(mappedLayout);

            // Act
            await _layoutService.UpdateAsync(layoutToUpdate);

            // Assert
            _layoutRepositoryMock.Verify(x => x.UpdateAsync(mappedLayout), Times.Once);
        }

        [Test]
        public async Task Update_LayoutNotFound_ThrowsValidationException()
        {
            // Arrange
            var notExistingId = 1;

            var layoutToUpdate = new LayoutModel { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(Layout));

            // Act
            var updatingEvent = _layoutService.Invoking(s => s.UpdateAsync(layoutToUpdate));

            // Assert
            await updatingEvent
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
            };

            var id = 2;

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layouts.First(l => l.Id == id));

            var layoutToUpdate = new LayoutModel { Id = 2, Description = "Layout 1", VenueId = 1, };
            var mappedLayout = new Layout { Id = 2, Description = "Layout 1", VenueId = 1, };

            _mapperMock.Setup(m => m.Map<Layout>(layoutToUpdate)).Returns(mappedLayout);

            // Act
            var updatingLayout = _layoutService.Invoking(s => s.UpdateAsync(layoutToUpdate));

            // Assert
            await updatingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public async Task Update_NullLayout_ThrowsValidationException()
        {
            // Arrange
            LayoutModel nullLayout = null;

            // Act
            var updatingLayout = _layoutService.Invoking(s => s.UpdateAsync(nullLayout));

            // Assert
            await updatingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Layout is null.");
        }

        [Test]
        public void GetAll_LayoutListNotEmpty_ReturnsLayoutList()
        {
            // Arrange
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
                new Layout { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            var mappedLayouts = new List<LayoutModel>
            {
                new LayoutModel { Id = 1, Description = "Layout 1", VenueId = 1, },
                new LayoutModel { Id = 2, Description = "Layout 2", VenueId = 1, },
                new LayoutModel { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            for (var i = 0; i < layouts.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<LayoutModel>(layouts[i])).Returns(mappedLayouts[i]);
            }

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            // Act
            var actualLayouts = _layoutService.GetAll();

            // Assert
            actualLayouts.Should().BeEquivalentTo(layouts);
        }

        [Test]
        public async Task GetById_LayoutExists_ReturnsLayout()
        {
            // Arrange
            var id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };
            var mappedLayout = new LayoutModel { Id = 1, Description = "Layout 1", VenueId = 1, };

            _mapperMock.Setup(m => m.Map<LayoutModel>(layout)).Returns(mappedLayout);

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);

            // Act
            var actualLayout = await _layoutService.GetByIdAsync(id);

            // Assert
            actualLayout.Should().BeEquivalentTo(layout);
        }

        [Test]
        public async Task GetById_LayoutNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Layout));

            // Act
            var gettingById = _layoutService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
