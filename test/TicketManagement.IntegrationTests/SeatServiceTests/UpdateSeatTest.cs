using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.SeatServiceTests
{
    internal class UpdateSeatTest
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
        public void Update_ValidSeat_UpdatesSeat()
        {
            int id = 1;

            var seatBeforeUpdate = new Seat
            {
                Id = id,
                Row = 1,
                Number = 1,
                AreaId = 1,
            };

            _seatService.GetById(id).Should().BeEquivalentTo(seatBeforeUpdate);

            var seatToUpdate = new Seat
            {
                Id = id,
                AreaId = 2,
                Row = 6,
                Number = 6,
            };

            _seatService.Update(seatToUpdate);

            _seatService.GetById(id).Should().BeEquivalentTo(seatToUpdate);
        }
    }
}
