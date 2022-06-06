using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.SeatServiceTests
{
    internal class CreateSeatTest
    {
        private ISeatService _seatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var seatRepo = new SeatSqlClientRepository(connectionString);
            var seatValidator = new SeatValidator(seatRepo);

            _seatService = new SeatService(seatRepo, seatValidator);
        }

        [Test]
        public void Create_ValidSeat_CreatesSeat()
        {
            var seatToCreate = new Seat
            {
                AreaId = 3,
                Row = 1,
                Number = 1,
            };

            var id = _seatService.Create(seatToCreate);

            _seatService.GetById(id).Should().BeEquivalentTo(seatToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
