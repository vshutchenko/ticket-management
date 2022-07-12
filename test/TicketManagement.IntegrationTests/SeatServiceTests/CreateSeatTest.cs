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
    internal class CreateSeatTest
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
        public async Task Create_ValidSeat_CreatesSeat()
        {
            Seat seatToCreate = new Seat
            {
                AreaId = 3,
                Row = 1,
                Number = 1,
            };

            var id = await _seatService.CreateAsync(seatToCreate);

            var actualSeat = await _seatService.GetByIdAsync(id);

            actualSeat.Should().BeEquivalentTo(seatToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
