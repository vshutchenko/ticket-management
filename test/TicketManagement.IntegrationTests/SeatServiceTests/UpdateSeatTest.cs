using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.SeatServiceTests
{
    internal class UpdateSeatTest
    {
        private ISeatService _seatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            SeatSqlClientRepository seatRepo = new SeatSqlClientRepository(connectionString);
            SeatValidator seatValidator = new SeatValidator(seatRepo);

            _seatService = new SeatService(seatRepo, seatValidator);
        }

        [Test]
        public async Task Update_ValidSeat_UpdatesSeat()
        {
            int id = 1;

            Seat seatBeforeUpdate = new Seat
            {
                Id = id,
                Row = 1,
                Number = 1,
                AreaId = 1,
            };

            BusinessLogic.Models.SeatModel actualSeatBeforeUpdate = await _seatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.Should().BeEquivalentTo(seatBeforeUpdate);

            Seat seatToUpdate = new Seat
            {
                Id = id,
                AreaId = 2,
                Row = 6,
                Number = 6,
            };

            await _seatService.UpdateAsync(seatToUpdate);

            BusinessLogic.Models.SeatModel actualSeatAfterUpdate = await _seatService.GetByIdAsync(id);

            actualSeatAfterUpdate.Should().BeEquivalentTo(seatToUpdate);
        }
    }
}
