using System;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.SeatServiceTests
{
    internal class DeleteSeatTest
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
