﻿using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.SqlClientImplementations.AreaServiceTests
{
    internal class DeleteAreaTest
    {
        private IAreaService _areaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var areaRepo = new AreaSqlClientRepository(connectionString);
            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var areaValidator = new AreaValidator(areaRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _areaService = new AreaService(areaRepo,  layoutRepo, areaValidator, mapper);
        }

        [Test]
        public async Task Delete_AreaExists_DeletesArea()
        {
            // Arrange
            var id = 1;

            // Act
            await _areaService.DeleteAsync(id);

            var gettingArea = _areaService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingArea
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
