using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Implementations;

namespace TicketManagement.IntegrationTests.LayoutServiceTests
{
    internal class UpdateLayoutTest
    {
        private ILayoutService _layoutService;

        [OneTimeSetUp]
        public void CreateServices()
        {
            var connectionString = new TestDatabase().ConnectionString;

            var layoutRepo = new LayoutSqlClientRepository(connectionString);
            var layoutValidator = new LayoutValidator(layoutRepo);

            _layoutService = new LayoutService(layoutRepo, layoutValidator);
        }

        [Test]
        public void Update_ValidLayout_UpdatesLayout()
        {
            int id = 1;

            var layoutBeforeUpdate = new Layout
            {
                Id = id,
                Description = "First layout",
                VenueId = 1,
            };

            _layoutService.GetById(id).Should().BeEquivalentTo(layoutBeforeUpdate);

            var layoutToUpdate = new Layout
            {
                Id = id,
                Description = "Test layout 1",
                VenueId = 1,
            };

            _layoutService.Update(layoutToUpdate);

            _layoutService.GetById(id).Should().BeEquivalentTo(layoutToUpdate);
        }
    }
}
