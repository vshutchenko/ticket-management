using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.SeatServiceTests
{
    internal class DeleteSeatTest
    {
        private ISeatService _seatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var seatRepo = new SeatSqlClientRepository(connectionString);
            var areaRepo = new AreaSqlClientRepository(connectionString);
            var seatValidator = new SeatValidator(seatRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _seatService = new SeatService(seatRepo, areaRepo, seatValidator, mapper);
        }

        [Test]
        public async Task Delete_SeatExists_DeletesSeat()
        {
            // Arrange
            var id = 1;

            // Act
            await _seatService.DeleteAsync(id);

            var gettingSeat = _seatService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingSeat
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
