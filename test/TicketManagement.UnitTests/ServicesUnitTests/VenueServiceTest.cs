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
    internal class VenueServiceTest
    {
        private Mock<IRepository<Venue>> _venueRepositoryMock;
        private IVenueService _venueService;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void SetUp()
        {
            _venueRepositoryMock = new Mock<IRepository<Venue>>();
            _mapperMock = new Mock<IMapper>();

            VenueValidator venueValidator = new VenueValidator(_venueRepositoryMock.Object);
            _venueService = new VenueService(_venueRepositoryMock.Object, venueValidator, _mapperMock.Object);
        }

        [Test]
        public async Task Create_ValidVenue_CreatesVenue()
        {
            // Arrange
            VenueModel venueToCreate = new VenueModel { Id = 1, Description = "New Venue", Address = "New Addres", Phone = "111 222 333 444" };

            Venue mappedVenue = new Venue { Id = 1, Description = "New Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _mapperMock.Setup(m => m.Map<Venue>(venueToCreate)).Returns(mappedVenue);

            // Act
            await _venueService.CreateAsync(venueToCreate);

            // Assert
            _venueRepositoryMock.Verify(x => x.CreateAsync(mappedVenue), Times.Once);
        }

        [Test]
        public async Task Create_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            List<Venue> venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
            };

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            VenueModel venueToCreate = new VenueModel { Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            Venue mappedVenue = new Venue { Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            _mapperMock.Setup(m => m.Map<Venue>(venueToCreate)).Returns(mappedVenue);

            // Act
            System.Func<Task<int>> creatingVenue = _venueService.Invoking(s => s.CreateAsync(venueToCreate));

            // Assert
            await creatingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Venue with same description is already exists.");
        }

        [Test]
        public async Task Create_NullVenue_ThrowsValidationException()
        {
            // Arrange
            VenueModel nullVenue = null;

            // Act
            System.Func<Task<int>> creatingVenue = _venueService.Invoking(s => s.CreateAsync(nullVenue));

            // Assert
            await creatingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Venue is null.");
        }

        [Test]
        public async Task Delete_VenueExists_DeletesVenue()
        {
            // Arrange
            Venue venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            int id = 1;

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(venue);

            // Act
            await _venueService.DeleteAsync(id);

            // Assert
            _venueRepositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
        }

        [Test]
        public async Task Delete_VenueNotFound_ThrowsValidationException()
        {
            // Arrange
            List<Venue> venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new Venue { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            int notExistingId = 99;

            // Act
            System.Func<Task> deletingVenue = _venueService.Invoking(s => s.DeleteAsync(notExistingId));

            // Assert
            await deletingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task UpdateDescription_ValidVenue_UpdatesVenue()
        {
            // Arrange
            int id = 1;

            Venue venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };
            List<Venue> venues = new List<Venue> { venue };

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(venue);
            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            VenueModel venueToUpdate = new VenueModel { Id = 1, Description = "Updated Venue", Address = "Addres 1", Phone = "111 222 333 444" };
            Venue mappedVenue = new Venue { Id = 1, Description = "Updated Venue", Address = "Addres 1", Phone = "111 222 333 444" };

            _mapperMock.Setup(m => m.Map<Venue>(venueToUpdate)).Returns(mappedVenue);

            // Act
            await _venueService.UpdateAsync(venueToUpdate);

            // Assert
            _venueRepositoryMock.Verify(x => x.UpdateAsync(mappedVenue), Times.Once);
        }

        [Test]
        public async Task UpdateAddressAndPhone_ValidVenue_UpdatesVenue()
        {
            // Arrange
            int id = 1;

            Venue venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };
            List<Venue> venues = new List<Venue> { venue };

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(venue);
            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            VenueModel venueToUpdate = new VenueModel { Id = 1, Description = "Venue 1", Address = "New Addres", Phone = "111 111 111 111" };
            Venue mappedVenue = new Venue { Id = 1, Description = "Venue 1", Address = "New Addres", Phone = "111 111 111 111" };

            _mapperMock.Setup(m => m.Map<Venue>(venueToUpdate)).Returns(mappedVenue);

            // Act
            await _venueService.UpdateAsync(venueToUpdate);

            // Assert
            _venueRepositoryMock.Verify(x => x.UpdateAsync(mappedVenue), Times.Once);
        }

        [Test]
        public async Task Update_VenueNotFound_ThrowsValidationException()
        {
            // Arrange
            int notExistingId = 1;

            VenueModel venueToUpdate = new VenueModel { Id = 1, Description = "Updated Venue", Address = "New Addres", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(notExistingId)).ReturnsAsync(default(Venue));

            // Act
            System.Func<Task> updatingVenue = _venueService.Invoking(s => s.UpdateAsync(venueToUpdate));

            // Assert
            await updatingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Update_VenueWithSameDescriptionExists_ThrowsValidationException()
        {
            // Arrange
            List<Venue> venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
            };

            int id = 2;

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(venues.First(v => v.Id == id));

            VenueModel venueToUpdate = new VenueModel { Id = 2, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };
            Venue mappedVenue = new Venue { Id = 2, Description = "Venue 1", Address = "New Addres", Phone = "111 222 333 444" };

            _mapperMock.Setup(m => m.Map<Venue>(venueToUpdate)).Returns(mappedVenue);

            // Act
            System.Func<Task> updatingVenue = _venueService.Invoking(s => s.UpdateAsync(venueToUpdate));

            // Assert
            await updatingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Venue with same description is already exists.");
        }

        [Test]
        public async Task Update_NullVenue_ThrowsValidationException()
        {
            // Arrange
            VenueModel nullVenue = null;

            // Act
            System.Func<Task> updatingVenue = _venueService.Invoking(s => s.UpdateAsync(nullVenue));

            // Assert
            await updatingVenue
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Venue is null.");
        }

        [Test]
        public void GetAll_VenueListNotEmpty_ReturnsVenueList()
        {
            // Arrange
            List<Venue> venues = new List<Venue>
            {
                new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new Venue { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new Venue { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            List<VenueModel> mappedVenues = new List<VenueModel>
            {
                new VenueModel { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" },
                new VenueModel { Id = 2, Description = "Venue 2", Address = "Addres 2", Phone = "555 566 333 333" },
                new VenueModel { Id = 3, Description = "Venue 3", Address = "Addres 3", Phone = "666 444 333 111" },
            };

            for (int i = 0; i < venues.Count; i++)
            {
                _mapperMock.Setup(m => m.Map<VenueModel>(venues[i])).Returns(mappedVenues[i]);
            }

            _venueRepositoryMock.Setup(x => x.GetAll()).Returns(venues.AsQueryable());

            // Act
            IEnumerable<VenueModel> actualVenues = _venueService.GetAll();

            // Assert
            actualVenues.Should().BeEquivalentTo(mappedVenues);
        }

        [Test]
        public async Task GetById_VenueExists_ReturnsVenue()
        {
            // Arrange
            int id = 1;

            Venue venue = new Venue { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            VenueModel mappedVenue = new VenueModel { Id = 1, Description = "Venue 1", Address = "Addres 1", Phone = "111 222 333 444" };

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(venue);

            _mapperMock.Setup(m => m.Map<VenueModel>(venue)).Returns(mappedVenue);

            // Act
            VenueModel actualVenue = await _venueService.GetByIdAsync(id);

            // Assert
            actualVenue.Should().BeEquivalentTo(mappedVenue);
        }

        [Test]
        public async Task GetById_VenueNotFound_ThrowsValidationException()
        {
            // Arrange
            int id = 1;

            _venueRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(default(Venue));

            // Act
            System.Func<Task<VenueModel>> gettingById = _venueService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
