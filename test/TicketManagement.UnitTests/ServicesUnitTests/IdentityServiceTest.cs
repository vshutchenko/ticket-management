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

        [Test]
        public async Task ChangePasswordAsync_ValidParameters_ChangesPassword()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var currentPassword = "password";
            var newPassword = "newpassword";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, currentPassword)).ReturnsAsync(true);

            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, currentPassword, newPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            await _identityService.ChangePasswordAsync(user.Id, currentPassword, newPassword);

            // Assert
            _userManagerMock.Verify(x => x.ChangePasswordAsync(user, currentPassword, newPassword), Times.Once);
        }

        [Test]
        public async Task ChangePasswordAsync_NotValidCurrentPassword_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var currentPassword = "password";
            var newPassword = "newpassword";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, user.PasswordHash)).ReturnsAsync(false);

            // Act
            var changingPassword = _identityService.Invoking(s => s.ChangePasswordAsync(user.Id, currentPassword, newPassword));

            // Assert
            await changingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Not valid current password.");
        }

        [Test]
        public async Task ChangePasswordAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            string id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";
            var currentPassword = "password";
            var newPassword = "newpassword";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var changingPassword = _identityService.Invoking(s => s.ChangePasswordAsync(id, currentPassword, newPassword));

            // Assert
            await changingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task GetUserAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserModel>(user)).Returns(userModel);

            // Act
            var actualUser = await _identityService.GetUserAsync(user.Id);

            // Assert
            actualUser.Should().BeEquivalentTo(userModel);
        }

        [Test]
        public async Task GetUserAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            string id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var gettingUser = _identityService.Invoking(s => s.GetUserAsync(id));

            // Assert
            await gettingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task GetRolesAsync_UserExists_ReturnsRoles()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var roles = new List<string> { "Role1", "Role2" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var actualRoles = await _identityService.GetRolesAsync(user.Id);

            // Assert
            actualRoles.Should().BeEquivalentTo(roles);
        }

        [Test]
        public async Task GetRolesAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            string id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var gettingRoles = _identityService.Invoking(s => s.GetRolesAsync(id));

            // Assert
            await gettingRoles
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task CreateUserAsync_ValidUser_CreatesUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(x => x.Map<User>(userModel)).Returns(user);

            // Act
            await _identityService.CreateUserAsync(userModel, password);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(user, password), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_ValidUser_AddsUserToUserRole()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";
            var userRole = "User";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.CreateAsync(user, password)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(x => x.Map<User>(userModel)).Returns(user);

            // Act
            await _identityService.CreateUserAsync(userModel, password);

            // Assert
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), userRole), Times.Once);
        }

        [Test]
        public async Task CreateUserAsync_UserAlreadyExists_ThrowsValidationException()
        {
            // Arrange
            var existingUser = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";

            _userManagerMock.Setup(x => x.FindByIdAsync(existingUser.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var creatingUser = _identityService.Invoking(s => s.CreateUserAsync(userModel, password));

            // Assert
            await creatingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User already exists.");
        }

        [Test]
        public async Task AssignRoleAsync_ValidParameters_AssignsRole()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";
            var role = "User";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, role)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _identityService.AssignRoleAsync(id, role);

            // Assert
            _userManagerMock.Verify(x => x.AddToRoleAsync(user, role), Times.Once);
        }

        [Test]
        public async Task AssignRoleAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";
            var role = "User";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var assigningRole = _identityService.Invoking(s => s.AssignRoleAsync(id, role));

            // Assert
            await assigningRole
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task AssignRoleAsync_RoleNotFound_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";
            var role = "User";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, role)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var assigningRole = _identityService.Invoking(s => s.AssignRoleAsync(id, role));

            // Assert
            await assigningRole
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Role was not found.");
        }

        public async Task AuthenticateAsync_ValidParameters_ReturnsUserModel()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var email = "email@mail.com";
            var password = "password";

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, password, false, false))
                .ReturnsAsync(SignInResult.Success);
            _mapperMock.Setup(x => x.Map<UserModel>(user)).Returns(userModel);

            // Act
            var actualUserModel = await _identityService.AuthenticateAsync(email, password);

            // Assert
            actualUserModel.Should().BeEquivalentTo(userModel);
        }

        [Test]
        public async Task AuthenticateAsync_EmailNotFound_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var email = "email@mail.com";
            var password = "password";

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));

            // Act
            var authenticating = _identityService.Invoking(s => s.AuthenticateAsync(email, password));

            // Assert
            await authenticating
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User with such email does not exists.");
        }

        [Test]
        public async Task AuthenticateAsync_WrongPassword_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var email = "email@mail.com";
            var password = "password";

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(user, password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var authenticating = _identityService.Invoking(s => s.AuthenticateAsync(email, password));

            // Assert
            await authenticating
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User with such email does not exists.");
        }
    }
}
