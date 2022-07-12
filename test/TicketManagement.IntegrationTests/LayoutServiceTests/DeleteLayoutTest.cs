﻿using FluentAssertions;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.SqlClientImplementations;

namespace TicketManagement.IntegrationTests.LayoutServiceTests
{
    internal class DeleteLayoutTest
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

        [TestCase(1)]
        public void Delete_LayoutExists_DeletesLayout(int id)
        {
            _layoutService.DeleteAsync(id);

            _layoutService.Invoking(s => s.GetByIdAsync(id))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Entity was not found.");
        }
    }
}
