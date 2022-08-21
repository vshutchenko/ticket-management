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
    internal class CreateAreaTest
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
        public async Task Create_ValidArea_CreatesArea()
        {
            // Arrange
            var areaToCreate = new AreaModel
            {
                Description = "Test area 1",
                CoordX = 1,
                CoordY = 1,
                LayoutId = 1,
            };

            // Act
            var id = await _areaService.CreateAsync(areaToCreate);

            var actualArea = await _areaService.GetByIdAsync(id);

            // Assert
            actualArea.Should().BeEquivalentTo(areaToCreate, opt => opt.Excluding(a => a.Id));
        }
    }
}
