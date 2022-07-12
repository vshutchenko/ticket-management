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
    internal class UpdateLayoutTest
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
        public async Task Update_ValidLayout_UpdatesLayout()
        {
            int id = 1;

            Layout layoutBeforeUpdate = new Layout
            {
                Id = id,
                Description = "First layout",
                VenueId = 1,
            };

            BusinessLogic.Models.LayoutModel actualLayoutBeforeUpdate = await _layoutService.GetByIdAsync(id);

            actualLayoutBeforeUpdate.Should().BeEquivalentTo(layoutBeforeUpdate);

            Layout layoutToUpdate = new Layout
            {
                Id = id,
                Description = "Test layout 1",
                VenueId = 1,
            };

            await _layoutService.UpdateAsync(layoutToUpdate);

            BusinessLogic.Models.LayoutModel layoutAfterUpdate = await _layoutService.GetByIdAsync(id);

            layoutAfterUpdate.Should().BeEquivalentTo(layoutToUpdate);
        }
    }
}
