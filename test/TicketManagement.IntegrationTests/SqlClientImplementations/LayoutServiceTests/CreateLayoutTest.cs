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

namespace TicketManagement.IntegrationTests.SqlClientImplementations.LayoutServiceTests
{
    internal class CreateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase.TestDatabase().ConnectionString;

            var layoutRepo = new LayoutSqlClientRepository(connectionString);
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
        public async Task Create_ValidLayout_CreatesLayout()
        {
            // Arrange
            var layoutToCreate = new LayoutModel
            {
                 Description = "Test layout 1",
                 VenueId = 2,
            };

            // Act
            var id = await _layoutService.CreateAsync(layoutToCreate);

            var actualLayout = await _layoutService.GetByIdAsync(id);

            // Assert
            actualLayout.Should().BeEquivalentTo(layoutToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
