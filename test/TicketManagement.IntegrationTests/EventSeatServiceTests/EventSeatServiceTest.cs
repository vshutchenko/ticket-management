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
        public void SetSeatState_ValidState_SetsState()
        {
            int id = 1;

            EventSeatState stateBeforeUpdate = EventSeatState.Ordered;

            _eventSeatService.GetById(id).State.Should().Be(stateBeforeUpdate);

            EventSeatState stateToUpdate = EventSeatState.Available;

            _eventSeatService.SetSeatState(id, stateToUpdate);

            _eventSeatService.GetById(id).State.Should().Be(stateToUpdate);
        }
    }
}
