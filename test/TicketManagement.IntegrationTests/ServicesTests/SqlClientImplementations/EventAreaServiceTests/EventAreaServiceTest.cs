using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.EventApi.MappingConfig;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.EventAreaServiceTests
{
    internal class EventAreaServiceTest
    {
        private IEventAreaService _eventAreaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var eventAreaRepo = new EventAreaSqlClientRepository(connectionString);
            var eventRepo = new EventSqlClientRepository(connectionString);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _eventAreaService = new EventAreaService(eventAreaRepo, eventRepo, new PriceValidator(), mapper);
        }

        [Test]
        public async Task SetPrice_ValidPrice_SetsPrice()
        {
            // Arrange
            var id = 1;

            decimal priceBeforeUpdate = 15;

            var eventAreaBeforeUpdate = await _eventAreaService.GetByIdAsync(id);

            eventAreaBeforeUpdate.Price.Should().Be(priceBeforeUpdate);

            decimal priceToUpdate = 10;

            // Act
            await _eventAreaService.SetPriceAsync(id, priceToUpdate);

            var eventAreaAfterUpdate = await _eventAreaService.GetByIdAsync(id);

            // Assert
            eventAreaAfterUpdate.Price.Should().Be(priceToUpdate);
        }
    }
}
