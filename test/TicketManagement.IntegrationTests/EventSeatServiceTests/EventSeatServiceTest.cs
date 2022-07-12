using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.EventSeatServiceTests
{
    internal class EventSeatServiceTest
    {
        private IEventSeatService _eventSeatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            EventSeatSqlClientRepository eventSeatRepo = new EventSeatSqlClientRepository(connectionString);

            _eventSeatService = new EventSeatService(eventSeatRepo);
        }

        [Test]
        public async Task SetSeatState_ValidState_SetsState()
        {
            int id = 1;

            EventSeatState stateBeforeUpdate = EventSeatState.Ordered;

            BusinessLogic.Models.EventSeatModel actualSeatBeforeUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.State.Should().Be(stateBeforeUpdate);

            EventSeatState stateToUpdate = EventSeatState.Available;

            await _eventSeatService.SetSeatStateAsync(id, stateToUpdate);

            BusinessLogic.Models.EventSeatModel actualSeatAfterUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatAfterUpdate.State.Should().Be(stateToUpdate);
        }
    }
}
