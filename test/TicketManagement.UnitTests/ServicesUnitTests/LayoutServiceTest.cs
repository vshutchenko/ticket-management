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

            _layoutService.Create(layoutToCreate);

            _layoutRepositoryMock.Verify(x => x.Create(layoutToCreate), Times.Once);
        }

        [Test]
        public void Create_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
            var layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
            };

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts);

            var layoutToCreate = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutService.Invoking(s => s.Create(layoutToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public void Create_NullLayout_ThrowsValidationException()
        {
            _layoutService.Invoking(s => s.Create(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Layout is null.");
        }

        [Test]
        public void Delete_LayoutExists_DeletesLayout()
        {
            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            int id = 1;

            _layoutRepositoryMock.Setup(x => x.GetById(id)).Returns(layout);

            _layoutService.Delete(id);

            _layoutRepositoryMock.Verify(x => x.Delete(id), Times.Once);
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

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts);

            int notExistingId = 99;

            _layoutService.Invoking(s => s.Delete(notExistingId))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_ValidLayout_UpdatesLayout()
        {
            int id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetById(id)).Returns(layout);
            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Layout> { layout });

            var layoutToUpdate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutService.Update(layoutToUpdate);

            _layoutRepositoryMock.Verify(x => x.Update(layoutToUpdate), Times.Once);
        }

        [Test]
        public void Update_LayoutNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var layoutToUpdate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetById(notExistingId)).Returns<Venue>(null);

            _layoutService.Invoking(s => s.Update(layoutToUpdate))
                .Should().Throw<ValidationException>()
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

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts);

            _layoutRepositoryMock.Setup(x => x.GetById(id)).Returns(layouts.First(l => l.Id == id));

            var layoutToUpdate = new Layout { Id = 2, Description = "Layout 1", VenueId = 1, };

            _layoutService.Invoking(s => s.Update(layoutToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("The same layout is already exists in current venue.");
        }

        [Test]
        public void Update_NullLayout_ThrowsValidationException()
        {
            _layoutService.Invoking(s => s.Update(null))
                .Should().Throw<ValidationException>()
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

            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(layouts);

            _layoutService.GetAll().Should().BeEquivalentTo(layouts);
        }

        [Test]
        public void GetById_LayoutExists_ReturnsLayout()
        {
            int id = 1;

            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetById(id)).Returns(layout);

            _layoutService.GetById(id).Should().BeEquivalentTo(layout);
        }

        [Test]
        public void GetById_LayoutNotFound_ThrowsValidationException()
        {
            int id = 1;

            _layoutRepositoryMock.Setup(x => x.GetById(id)).Returns<Layout>(null);

            _layoutService.Invoking(s => s.GetById(id))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
