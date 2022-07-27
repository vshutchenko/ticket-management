﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.EntityFrameworkImplementations;

namespace TicketManagement.IntegrationTests.EFImplemetations.SeatServiceTests
{
    internal class CreateSeatTest
    {
        private ISeatService _seatService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var seatRepo = new SeatRepository(context);
            var seatValidator = new SeatValidator(seatRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _seatService = new SeatService(seatRepo, seatValidator, mapper);
        }

        [Test]
        public async Task Create_ValidSeat_CreatesSeat()
        {
            // Arrange
            var seatToCreate = new SeatModel
            {
                AreaId = 3,
                Row = 1,
                Number = 1,
            };

            // Act
            var id = await _seatService.CreateAsync(seatToCreate);

            var actualSeat = await _seatService.GetByIdAsync(id);

            // Assert
            actualSeat.Should().BeEquivalentTo(seatToCreate, v => v.Excluding(v => v.Id));
        }
    }
}