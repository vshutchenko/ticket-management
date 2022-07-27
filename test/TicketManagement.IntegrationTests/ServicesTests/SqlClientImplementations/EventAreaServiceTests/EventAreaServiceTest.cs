using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.EventAreaServiceTests
{
    internal class EventAreaServiceTest
    {
        private IEventAreaService _eventAreaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var eventAreaRepo = new EventAreaSqlClientRepository(connectionString);
            var eventRepo = new EventSqlClientRepository(connectionString);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
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
