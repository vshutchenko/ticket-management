using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.EventApi.MappingConfig;
using TicketManagement.EventApi.Models;
using TicketManagement.EventApi.Services.Implementations;
using TicketManagement.EventApi.Services.Interfaces;
using TicketManagement.EventApi.Services.Validation;

namespace TicketManagement.IntegrationTests.EFImplemetations.EventServiceTests
{
    internal class DeleteEventTest
    {
        private IEventService _eventService;
        private IEventAreaService _eventAreaService;
        private IEventSeatService _eventSeatService;

        [SetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var eventRepo = new EventRepository(context);
            var areaRepo = new AreaRepository(context);
            var seatRepo = new SeatRepository(context);

            var eventAreaRepo = new EventAreaRepository(context);
            var eventSeatRepo = new EventSeatRepository(context);

            var eventValidator = new EventValidator(eventRepo, seatRepo, areaRepo);
            var priceValidator = new PriceValidator();

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _eventSeatService = new EventSeatService(eventSeatRepo, eventAreaRepo, mapper);
            _eventAreaService = new EventAreaService(eventAreaRepo, eventRepo, priceValidator, mapper);

            _eventService = new EventService(eventRepo, eventValidator, _eventSeatService, _eventAreaService, mapper);
        }

        [Test]
        public async Task Delete_EventExists_DeletesEvent()
        {
            // Arrange
            var id = 1;

            // Act
            await _eventService.DeleteAsync(id);

            var gettingById = _eventService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingById
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }

        [Test]
        public async Task Delete_EventExists_DeletesAreas()
        {
            // Arrange
            var id = 1;
            var expectedEventAreasCount = 0;

            // Act
            await _eventService.DeleteAsync(id);

            var actualEventAreasCount = _eventAreaService
                .GetAll()
                .Count(a => a.EventId == id);

            // Assert
            actualEventAreasCount.Should().Be(expectedEventAreasCount);
        }

        [Test]
        public async Task Delete_EventExists_DeletesSeats()
        {
            // Arrange
            var id = 1;
            var expectedEventSeatsCount = 0;

            // Act
            await _eventService.DeleteAsync(id);

            var actualEventSeatsCount = _eventAreaService
                .GetAll()
                .Where(a => a.EventId == id)
                .Sum(a => _eventSeatService.GetAll().Count(s => s.EventAreaId == a.Id));

            // Assert
            expectedEventSeatsCount.Should().Be(actualEventSeatsCount);
        }
    }
}
