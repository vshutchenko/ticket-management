using System;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.SeatServiceTests
{
    internal class DeleteSeatTest
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

        [TestCase(1)]
        public void Delete_SeatExists_DeletesSeat(int id)
        {
            _seatService.DeleteAsync(id);

            _seatService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
