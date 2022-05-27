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

        [TestCase(1, EventSeatState.Available)]
        [TestCase(2, EventSeatState.Ordered)]
        public void SetSeatState_StateChanged(int id, EventSeatState state)
        {
            _eventSeatService.SetSeatState(id, state);

            var actualState = _eventSeatService.GetById(id).State;

            Assert.AreEqual(state, actualState);
        }
    }
}
