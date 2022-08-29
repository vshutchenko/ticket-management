using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.SeatServiceTests
{
    internal class CreateSeatTest
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
        public async Task Create_ValidSeat_CreatesSeat()
        {
            // Arrange
            var seatToCreate = new SeatModel
            {
                AreaId = 3,
                Row = 1,
                Number = 1,
            };

            // Act
            var id = await _seatService.CreateAsync(seatToCreate);

            var actualSeat = await _seatService.GetByIdAsync(id);

            // Assert
            actualSeat.Should().BeEquivalentTo(seatToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
