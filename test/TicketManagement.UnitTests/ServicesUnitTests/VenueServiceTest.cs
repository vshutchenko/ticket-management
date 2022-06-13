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

        [SetUp]
        public void SetUp()
        {
            _venueRepositoryMock = new Mock<IRepository<Venue>>();

            var venueValidator = new VenueValidator(_venueRepositoryMock.Object);

            _venueService = new VenueService(_venueRepositoryMock.Object, venueValidator);
        }

        [Test]
        public void Create_ValidVenue_CreatesVenue()
        {
            var venueToCreate = new Venue { Id = 1, Description = "New Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _venueService.Create(venueToCreate);

            _venueRepositoryMock.Verify(x => x.Create(venueToCreate), Times.Once);
        }

        [Test]
        public void Create_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            var venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
            };

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues);

            var venueToCreate = new Venue { Id = 1, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            _venueService.Invoking(s => s.Create(venueToCreate))
                .Should().Throw<ValidationException>()
                .WithMessage("Venue with same description is already exists.");
        }

        [Test]
        public void Create_NullVenue_ThrowsValidationException()
        {
            _venueService.Invoking(s => s.Create(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Venue is null.");
        }

        [Test]
        public void Delete_VenueExists_DeletesVenue()
        {
            var venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            int id = 1;

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns(venue);

            _venueService.Delete(id);

            _venueRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [Test]
        public void Delete_VenueNotFound_ThrowsValidationException()
        {
            var venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new Venue { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues);

            int notExistingId = 99;

            _venueService.Invoking(s => s.Delete(notExistingId))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void UpdateDescription_ValidVenue_UpdatesVenue()
        {
            int id = 1;

            var venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns(venue);
            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Venue> { venue });

            var venueToUpdate = new Venue { Id = 1, Description = "Updated Venue", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueService.Update(venueToUpdate);

            _venueRepositoryMock.Verify(x => x.Update(venueToUpdate), Times.Once);
        }

        [Test]
        public void UpdateAddressAndPhone_ValidVenue_UpdatesVenue()
        {
            int id = 1;

            var venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns(venue);
            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Venue> { venue });

            var venueToUpdate = new Venue { Id = 1, Description = "Venue 1", Address = "New Addres", Phone = "111 111 111 111" };

            _venueService.Update(venueToUpdate);

            _venueRepositoryMock.Verify(x => x.Update(venueToUpdate), Times.Once);
        }

        [Test]
        public void Update_VenueNotFound_ThrowsValidationException()
        {
            int notExistingId = 1;

            var venueToUpdate = new Venue { Id = 1, Description = "Updated Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetById(notExistingId)).Returns<Venue>(null);

            _venueService.Invoking(s => s.Update(venueToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public void Update_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            var venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
            };

            int id = 2;

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues);

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns(venues.First(v => v.Id == id));

            var venueToUpdate = new Venue { Id = 2, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            _venueService.Invoking(s => s.Update(venueToUpdate))
                .Should().Throw<ValidationException>()
                .WithMessage("Venue with same description is already exists.");
        }

        [Test]
        public void Update_NullVenue_ThrowsValidationException()
        {
            _venueService.Invoking(s => s.Update(null))
                .Should().Throw<ValidationException>()
                .WithMessage("Venue is null.");
        }

        [Test]
        public void GetAll_VenueListNotEmpty_ReturnsVenueList()
        {
            var venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new Venue { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues);

            _venueService.GetAll().Should().BeEquivalentTo(venues);
        }

        [Test]
        public void GetById_VenueExists_ReturnsVenue()
        {
            int id = 1;

            var venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns(venue);

            _venueService.GetById(id).Should().BeEquivalentTo(venue);
        }

        [Test]
        public void GetById_VenueNotFound_ThrowsValidationException()
        {
            int id = 1;

            _venueRepositoryMock.Setup(x => x.GetById(id)).Returns<Venue>(null);

            _venueService.Invoking(s => s.GetById(id))
                .Should().Throw<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
