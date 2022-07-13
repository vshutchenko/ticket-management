using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.MappingConfig;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.AreaServiceTests
{
    internal class DeleteAreaTest
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
        public async Task Delete_AreaExists_DeletesArea()
        {
            // Arrange
            int id = 1;

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
