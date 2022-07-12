using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using TicketManagement.BusinessLogic.Implementations;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class IdentityServiceTest
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<SignInManager<User>> _signInManagerMock;
        private Mock<IMapper> _mapperMock;
        private IIdentityService _identityService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                 new Mock<IHttpContextAccessor>().Object,
                 new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                 new Mock<IOptions<IdentityOptions>>().Object,
                 new Mock<ILogger<SignInManager<User>>>().Object,
                 new Mock<IAuthenticationSchemeProvider>().Object);

            _mapperMock = new Mock<IMapper>();

            _identityService = new IdentityService(_userManagerMock.Object, _roleManagerMock.Object, _signInManagerMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task UpdateUserAsync_ValidParameters_UpdatesUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _identityService.UpdateUserAsync(userModel);

            // Assert
            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));

            // Act
            var updatingUser = _identityService.Invoking(s => s.UpdateUserAsync(userModel));

            // Assert
            await updatingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task UpdateUserAsync_EmailIsAlreadyTaken_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userToUpdate = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "newMail@gmail.com" };

            var newUserWithExistingEmail = new User { Id = "22fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "newMail@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(userToUpdate.Email)).ReturnsAsync(newUserWithExistingEmail);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var updatingUser = _identityService.Invoking(s => s.UpdateUserAsync(userToUpdate));

            // Assert
            await updatingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("This email is already taken.");
        }
    }
}
