﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.Core.Models;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.EventApi.MappingConfig;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.EventSeatServiceTests
{
    internal class EventSeatServiceTest
    {
        private IEventSeatService _eventSeatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var eventSeatRepo = new EventSeatSqlClientRepository(connectionString);
            var eventAreaRepo = new EventAreaSqlClientRepository(connectionString);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _eventSeatService = new EventSeatService(eventSeatRepo, eventAreaRepo, mapper);
        }

        [Test]
        public async Task SetSeatState_ValidState_SetsState()
        {
            // Arrange
            var id = 1;

            var stateBeforeUpdate = EventSeatState.Available;

            var actualSeatBeforeUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.State.Should().Be(stateBeforeUpdate);

            var stateToUpdate = EventSeatState.Ordered;

            // Act
            await _eventSeatService.SetSeatStateAsync(id, stateToUpdate);

            var actualSeatAfterUpdate = await _eventSeatService.GetByIdAsync(id);

            // Assert
            actualSeatAfterUpdate.State.Should().Be(stateToUpdate);
        }
    }
}
