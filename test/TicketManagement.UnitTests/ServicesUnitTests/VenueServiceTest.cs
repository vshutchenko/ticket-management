using System;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    internal class VenueServiceTest
    {
        private Mock<IRepository<Venue>> _venueRepositoryMock;
        private Mock<IValidator<Venue>> _venueValidatorMock;
        private Mock<IValidator<Venue>> _venueValidatorMockThrowsException;

        [SetUp]
        public void SetUp()
        {
            _venueRepositoryMock = new Mock<IRepository<Venue>>();

            _venueValidatorMock = new Mock<IValidator<Venue>>();

            _venueValidatorMockThrowsException = new Mock<IValidator<Venue>>();
            _venueValidatorMockThrowsException.Setup(x => x.Validate(It.IsAny<Venue>())).Throws<ValidationException>();
        }

        [Test]
        public void Create_ValidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            venueService.Create(new Venue());

            _venueValidatorMock.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Create(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Create_InvalidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMockThrowsException.Object);

            Assert.Throws<ValidationException>(() => venueService.Create(new Venue()));
        }

        [Test]
        public void Create_NullVenue_ThrowsAgrumentNullException()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMockThrowsException.Object);

            Assert.Throws<ArgumentNullException>(() => venueService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            venueService.Delete(id);

            _venueRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => venueService.Delete(id));
            _venueRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            venueService.Update(new Venue());

            _venueValidatorMock.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Update(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Update_InvalidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMockThrowsException.Object);

            Assert.Throws<ValidationException>(() => venueService.Update(new Venue()));
        }

        [Test]
        public void Update_NullVenue_ThrowsAgrumentNullException()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMockThrowsException.Object);

            Assert.Throws<ArgumentNullException>(() => venueService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            venueService.GetAll();

            _venueRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            venueService.GetById(id);

            _venueRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidatorMock.Object);

            Assert.Throws<ArgumentException>(() => venueService.GetById(id));
        }
    }
}
