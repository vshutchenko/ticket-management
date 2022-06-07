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
        private List<Layout> _layouts;

        [SetUp]
        public void SetUp()
        {
            _layouts = new List<Layout>
            {
                new Layout { Id = 1, Description = "Layout 1", VenueId = 1, },
                new Layout { Id = 2, Description = "Layout 2", VenueId = 1, },
                new Layout { Id = 3, Description = "Layout 3", VenueId = 2, },
            };

            _layoutRepositoryMock = new Mock<IRepository<Layout>>();
            _layoutRepositoryMock.Setup(x => x.GetAll()).Returns(_layouts);

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
            _layoutService.Delete(It.IsAny<int>());

            _layoutRepositoryMock.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Update_ValidLayout_UpdatesLayout()
        {
            var layoutToUpdate = new Layout { Id = 1, Description = "New Layout", VenueId = 1, };

            _layoutService.Update(layoutToUpdate);

            _layoutRepositoryMock.Verify(x => x.Update(layoutToUpdate), Times.Once);
        }

        [Test]
        public void Update_LayoutWithSameDescriptionExists_ThrowsValidationException()
        {
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
            var layouts = _layoutService.GetAll().ToList();

            layouts.Should().BeEquivalentTo(_layouts);
        }

        [Test]
        public void GetById_LayoutExists_ReturnsLayout()
        {
            var layout = new Layout { Id = 1, Description = "Layout 1", VenueId = 1, };

            _layoutRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(layout);

            _layoutService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(layout);
        }

        [Test]
        public void GetById_LayoutNotFound_ReturnsNull()
        {
            _layoutRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Layout>(null);

            _layoutService.GetById(It.IsAny<int>()).Should().BeNull();
        }
    }
}
