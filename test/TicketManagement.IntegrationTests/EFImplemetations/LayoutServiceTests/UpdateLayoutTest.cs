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

namespace TicketManagement.IntegrationTests.EFImplemetations.LayoutServiceTests
{
    internal class UpdateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var layoutRepo = new LayoutRepository(context);
            var layoutValidator = new LayoutValidator(layoutRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
                })
                .CreateMapper();

            _layoutService = new LayoutService(layoutRepo, layoutValidator, mapper);
        }

        [Test]
        public async Task Update_ValidLayout_UpdatesLayout()
        {
            // Arrange
            var id = 1;

            var layoutBeforeUpdate = new LayoutModel
            {
                Id = id,
                Description = "First layout",
                VenueId = 1,
            };

            var actualLayoutBeforeUpdate = await _layoutService.GetByIdAsync(id);

            actualLayoutBeforeUpdate.Should().BeEquivalentTo(layoutBeforeUpdate);

            var layoutToUpdate = new LayoutModel
            {
                Id = id,
                Description = "Test layout 1",
                VenueId = 1,
            };

            // Act
            await _layoutService.UpdateAsync(layoutToUpdate);

            var layoutAfterUpdate = await _layoutService.GetByIdAsync(id);

            // Assert
            layoutAfterUpdate.Should().BeEquivalentTo(layoutToUpdate);
        }
    }
}