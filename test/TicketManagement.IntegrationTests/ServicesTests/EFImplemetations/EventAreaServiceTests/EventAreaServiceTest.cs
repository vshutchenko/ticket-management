using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.EventApi.MappingConfig;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.IntegrationTests.EFImplemetations.EventAreaServiceTests
{
    internal class EventAreaServiceTest
    {
        private IEventAreaService _eventAreaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var eventAreaRepo = new EventAreaRepository(context);
            var eventRepo = new EventRepository(context);

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
