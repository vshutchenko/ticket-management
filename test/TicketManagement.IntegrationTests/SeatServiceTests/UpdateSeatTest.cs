using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
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
            var connectionString = new TestDatabase().ConnectionString;

            var seatRepo = new SeatSqlClientRepository(connectionString);
            var seatValidator = new SeatValidator(seatRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _seatService = new SeatService(seatRepo, seatValidator, mapper);
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
