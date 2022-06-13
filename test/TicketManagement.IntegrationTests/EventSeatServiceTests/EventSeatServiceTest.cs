using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.EventSeatServiceTests
{
    internal class EventSeatServiceTest
    {
        private IEventSeatService _eventSeatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var eventSeatRepo = new EventSeatSqlClientRepository(connectionString);

            _eventSeatService = new EventSeatService(eventSeatRepo);
        }

        [Test]
        public async Task SetSeatState_ValidState_SetsState()
        {
            int id = 1;

            EventSeatState stateBeforeUpdate = EventSeatState.Ordered;

            var actualSeatBeforeUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.State.Should().Be(stateBeforeUpdate);

            EventSeatState stateToUpdate = EventSeatState.Available;

            await _eventSeatService.SetSeatStateAsync(id, stateToUpdate);

            var actualSeatAfterUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatAfterUpdate.State.Should().Be(stateToUpdate);
        }
    }
}
