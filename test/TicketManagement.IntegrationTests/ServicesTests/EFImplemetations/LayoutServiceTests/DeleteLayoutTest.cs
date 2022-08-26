using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.VenueApi.MappingConfig;
using TicketManagement.VenueApi.Models;
using TicketManagement.VenueApi.Services.Implementations;
using TicketManagement.VenueApi.Services.Interfaces;
using TicketManagement.VenueApi.Services.Validation;

namespace TicketManagement.IntegrationTests.EFImplemetations.LayoutServiceTests
{
    internal class DeleteLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var testDbInfo = new TestDatabase.TestDatabaseInfo();
            var connectionString = testDbInfo.ConnectionString;
            testDbInfo.CreateDb();

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var layoutRepo = new LayoutRepository(context);
            var venueRepo = new VenueRepository(context);
            var eventRepo = new EventRepository(context);
            var layoutValidator = new LayoutValidator(layoutRepo);

            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _layoutService = new LayoutService(layoutRepo, venueRepo, eventRepo, layoutValidator, mapper);
        }

        [Test]
        public async Task Delete_LayoutHostsEvent_ThrowsValidationException()
        {
            // Arrange
            var id = 1;

            // Act
            var deletingLayout = _layoutService.Invoking(s => s.DeleteAsync(id));

            // Assert
            await deletingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("This layout cannot be deleted as it will host an event.");
        }

        [Test]
        public async Task Delete_LayoutExists_DeletesLayout()
        {
            // Arrange
            var id = 2;

            // Act
            await _layoutService.DeleteAsync(id);

            var gettingLayout = _layoutService.Invoking(s => s.GetByIdAsync(id));

            // Assert
            await gettingLayout
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
