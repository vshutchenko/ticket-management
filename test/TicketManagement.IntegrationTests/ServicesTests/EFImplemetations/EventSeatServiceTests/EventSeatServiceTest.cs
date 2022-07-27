﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.DataAccess.EntityFrameworkImplementations;

namespace TicketManagement.IntegrationTests.EFImplemetations.EventSeatServiceTests
{
    internal class EventSeatServiceTest
    {
        private IEventSeatService _eventSeatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var eventSeatRepo = new EventSeatRepository(context);
            var eventAreaRepo = new EventAreaRepository(context);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _eventSeatService = new EventSeatService(eventSeatRepo, eventAreaRepo, mapper);
        }

        [Test]
        public async Task SetSeatState_ValidState_SetsState()
        {
            // Arrange
            var id = 1;

            var stateBeforeUpdate = EventSeatStateModel.Available;

            var actualSeatBeforeUpdate = await _eventSeatService.GetByIdAsync(id);

            actualSeatBeforeUpdate.State.Should().Be(stateBeforeUpdate);

            var stateToUpdate = EventSeatStateModel.Ordered;

            // Act
            await _eventSeatService.SetSeatStateAsync(id, stateToUpdate);

            var actualSeatAfterUpdate = await _eventSeatService.GetByIdAsync(id);

            // Assert
            actualSeatAfterUpdate.State.Should().Be(stateToUpdate);
        }
    }
}