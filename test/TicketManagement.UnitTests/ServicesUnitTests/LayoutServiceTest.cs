using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    internal class LayoutServiceTest
    {
        private Mock<IRepository<Layout>> _layoutRepositoryMock;
        private ILayoutService _layoutService;

        [SetUp]
        public void SetUp()
        {
            _layoutRepositoryMock = new Mock<IRepository<Layout>>();

            var layoutValidator = new LayoutValidator(_layoutRepositoryMock.Object);

            _layoutService = new LayoutService(_layoutRepositoryMock.Object, layoutValidator);
        }

        [Test]
        public void Create_ValidLayout_CreatesLayout()
        {
            var layoutToCreate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutService.CreateAsync(layoutToCreate);

            _layoutRepositoryMock.Verify(x => x.CreateAsync(layoutToCreate), Times.Once);
        }

        [Test]
        public void Create_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            var layoutToCreate = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutService.Invoking(s => s.CreateAsync(layoutToCreate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public void Create_NullLayout_ThrowsValidationException()
        {
            _layoutService.Invoking(s => s.CreateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Layout is null.");
        }

        [Test]
        public void Delete_LayoutExists_DeletesLayout()
        {
            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            int id = 1;

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);

            _layoutService.DeleteAsync(id);

            _layoutRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public void Delete_LayoutNotFound_ThrowsValidationException()
        {
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
                new Layout { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            int notExistingId = 99;

            _layoutService.Invoking(s => s.DeleteAsync(notExistingId))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_ValidLayout_UpdatesLayout()
        {
            int id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };
            var layouts = new List<Layout> { layout };

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);
            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            var layoutToUpdate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutService.UpdateAsync(layoutToUpdate);

            _layoutRepositoryMock.Verify(x => x.UpdateAsync(layoutToUpdate), Times.Once);
        }

        [Test]
        public void Update_LayoutNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var layoutToUpdate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).Returns<Venue>(null);

            _layoutService.Invoking(s => s.UpdateAsync(layoutToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
            };

            int id = 2;

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layouts.First(l => l.Id == id));

            var layoutToUpdate = new Layout { Id = 2, Description = "Layout 1", VenueId = 1, };

            _layoutService.Invoking(s => s.UpdateAsync(layoutToUpdate))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public void Update_NullLayout_ThrowsValidationException()
        {
            _layoutService.Invoking(s => s.UpdateAsync(null))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Layout is null.");
        }

        [Test]
        public void GetAll_LayoutListNotEmpty_ReturnsLayoutList()
        {
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
                new Layout { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts.AsQueryable());

            _layoutService.GetAll().Should().BeEquivalentTo(layouts);
        }

        [Test]
        public async Task GetById_LayoutExists_ReturnsLayout()
        {
            int id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(layout);

            var actualLayout = await _layoutService.GetByIdAsync(id);

            actualLayout.Should().BeEquivalentTo(layout);
        }

        [Test]
        public void GetById_LayoutNotFound_ThrowsValidationException()
        {
            int id = 1;

            _layoutRepositoryMock.Setup(x => x.GetByIdAsync(id)).Returns<Layout>(null);

            _layoutService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
