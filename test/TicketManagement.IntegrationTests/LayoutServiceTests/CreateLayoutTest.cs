using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.LayoutServiceTests
{
    internal class CreateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            string connectionString = new TestDatabase().ConnectionString;

            LayoutSqlClientRepository layoutRepo = new LayoutSqlClientRepository(connectionString);
            LayoutValidator layoutValidator = new LayoutValidator(layoutRepo);

            _layoutService = new LayoutService(layoutRepo, layoutValidator);
        }

        [Test]
        public async Task Create_ValidLayout_CreatesLayout()
        {
            Layout layoutToCreate = new Layout
            {
                 Description = "Test layout 1",
                 VenueId = 2,
            };

            var id = await _layoutService.CreateAsync(layoutToCreate);

            var actualLayout = await _layoutService.GetByIdAsync(id);

            actualLayout.Should().BeEquivalentTo(layoutToCreate, v => v.Excluding(v => v.Id));
        }
    }
}
