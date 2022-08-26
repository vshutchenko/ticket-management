using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.EFImplemetations.SeatServiceTests
{
    internal class UpdateSeatTest
    {
        private ISeatService _seatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var seatRepo = new SeatRepository(context);
            var areaRepo = new AreaRepository(context);
            var seatValidator = new SeatValidator(seatRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _seatService = new SeatService(seatRepo, areaRepo, seatValidator, mapper);
        }

        [Test]
        public async Task Update_ValidSeat_UpdatesSeat()
        {
            // Arrange
            var id = 1;

            var seatBeforeUpdate = new SeatModel
            {
                Id = id,
                Row = 1,
                Number = 1,
                AreaId = 1,
            };

            var actualSeatBeforeUpdate = await _seatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.Should().BeEquivalentTo(seatBeforeUpdate);

            var seatToUpdate = new SeatModel
            {
                Id = id,
                AreaId = 2,
                Row = 6,
                Number = 6,
            };

            // Act
            await _seatService.UpdateAsync(seatToUpdate);

            var actualSeatAfterUpdate = await _seatService.GetByIdAsync(id);

            // Assert
            actualSeatAfterUpdate.Should().BeEquivalentTo(seatToUpdate);
        }
    }
}
