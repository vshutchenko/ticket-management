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
        private Mock<IValidationService<Venue>> _venueValidationMockAlwaysTrue;
        private Mock<IValidationService<Venue>> _venueValidationMockAlwaysFalse;

        [SetUp]
        public void SetUp()
        {
            _venueRepositoryMock = new Mock<IRepository<Venue>>();

            _venueValidationMockAlwaysTrue = new Mock<IValidationService<Venue>>();
            _venueValidationMockAlwaysTrue.Setup(x => x.Validate(It.IsAny<Venue>())).Returns(true);

            _venueValidationMockAlwaysFalse = new Mock<IValidationService<Venue>>();
            _venueValidationMockAlwaysFalse.Setup(x => x.Validate(It.IsAny<Venue>())).Returns(false);
        }

        [Test]
        public void Create_ValidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            venueService.Create(new Venue());

            _venueValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Create(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Create_InvalidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysFalse.Object);

            venueService.Create(new Venue());

            _venueValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Create(It.IsAny<Venue>()), Times.Never);
        }

        [Test]
        public void Create_NullVenue_ThrowsAgrumentNullException()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => venueService.Create(null));
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void Delete_ValidId(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            venueService.Delete(id);

            _venueRepositoryMock.Verify(x => x.Delete(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Delete_InvalidId_ThrowsAgrumentException(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => venueService.Delete(id));
            _venueRepositoryMock.Verify(x => x.Delete(id), Times.Never);
        }

        [Test]
        public void Update_ValidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            venueService.Update(new Venue());

            _venueValidationMockAlwaysTrue.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Update(It.IsAny<Venue>()), Times.Once);
        }

        [Test]
        public void Update_InvalidVenue()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysFalse.Object);

            venueService.Update(new Venue());

            _venueValidationMockAlwaysFalse.Verify(x => x.Validate(It.IsAny<Venue>()), Times.Once);
            _venueRepositoryMock.Verify(x => x.Update(It.IsAny<Venue>()), Times.Never);
        }

        [Test]
        public void Update_NullVenue_ThrowsAgrumentNullException()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysFalse.Object);

            Assert.Throws<ArgumentNullException>(() => venueService.Update(null));
        }

        [Test]
        public void GetAll_Test()
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            venueService.GetAll();

            _venueRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [TestCase(1)]
        [TestCase(int.MaxValue)]
        public void GetById_Test(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            venueService.GetById(id);

            _venueRepositoryMock.Verify(x => x.GetById(id), Times.Once);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void GetById_InvalidId_ThrowsArgumentException(int id)
        {
            IVenueService venueService = new VenueService(_venueRepositoryMock.Object, _venueValidationMockAlwaysTrue.Object);

            Assert.Throws<ArgumentException>(() => venueService.GetById(id));
        }
    }
}
