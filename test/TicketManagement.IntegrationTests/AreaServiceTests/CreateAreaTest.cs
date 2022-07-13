using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.AreaServiceTests
{
    internal class CreateAreaTest
    {
        private IAreaService _areaService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var areaRepo = new AreaSqlClientRepository(connectionString);
            var areaValidator = new AreaValidator(areaRepo);
            var mapper = new MapperConfiguration(mc =>
                {
                    mc.AddProfile(new MappingProfile());
                    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
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
