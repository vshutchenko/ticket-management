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

namespace TicketManagement.IntegrationTests.EFImplemetations.AreaServiceTests
{
    internal class DeleteAreaTest
    {
        private IAreaService _areaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var optionsBuilder = new DbContextOptionsBuilder<TicketManagementContext>();
            optionsBuilder.UseSqlServer(connectionString);

            var context = new TicketManagementContext(optionsBuilder.Options);

            var areaRepo = new AreaRepository(context);
            var areaValidator = new AreaValidator(areaRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                })
                .CreateMapper();

            _areaService = new AreaService(areaRepo, areaValidator, mapper);
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
