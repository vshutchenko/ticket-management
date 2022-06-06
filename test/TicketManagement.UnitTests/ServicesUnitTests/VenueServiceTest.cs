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
    internal class VenueServiceTest
    {
        private Mock<IRepository<Venue>> _venueRepositoryMock;
        private IVenueService _venueService;
        private List<Venue> _venues;

        [SetUp]
        public void SetUp()
        {
            _venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new Venue { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            _venueRepositoryMock = new Mock<IRepository<Venue>>();

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(_venues);

            var venueValidator = new VenueValidator(_venueRepositoryMock.Object);

            _venueService = new VenueService(_venueRepositoryMock.Object, venueValidator);
        }

        [Test]
        public void Create_ValidVenue_CreatesVenue()
        {
            var venueToCreate = new Venue { Id = 1, Description = "New Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _venueService.Create(venueToCreate);

            _venueRepositoryMock.Verify(x => x.Create(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Create_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            var venueToCreate = new Venue { Id = 1, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            Assert.Throws<ValidationException>(() => _venueService.Create(venueToCreate));
        }

        [Test]
        public void Create_NullVenue_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _venueService.Create(null));
        }

        [Test]
        public void Delete_VenueExists_DeletesVenue()
        {
            _venueService.Delete(It.IsAny<int>());

            _venueRepositoryMock.Verify(x => x.Delete(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void Update_ValidVenue_UpdatesVenue()
        {
            var venueToUpdate = new Venue { Id = 1, Description = "Updated Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _venueService.Update(venueToUpdate);

            _venueRepositoryMock.Verify(x => x.Update(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Update_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            var venueToUpdate = new Venue { Id = 2, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            Assert.Throws<ValidationException>(() => _venueService.Update(venueToUpdate));
        }

        [Test]
        public void Update_NullVenue_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(() => _venueService.Update(null));
        }

        [Test]
        public void GetAll_VenueListNotEmpty_ReturnsVenueList()
        {
            var venues = _venueService.GetAll().ToList();

            venues.Should().BeEquivalentTo(_venues);
        }

        [Test]
        public void GetById_VenueExists_ReturnsVenue()
        {
            var venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(venue);

            _venueService.GetById(It.IsAny<int>()).Should().BeEquivalentTo(venue);
        }

        [Test]
        public void GetById_VenueNotFound_ReturnsNull()
        {
            _venueRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<Venue>(null);

            Assert.IsNull(_venueService.GetById(It.IsAny<int>()));
        }
    }
}
