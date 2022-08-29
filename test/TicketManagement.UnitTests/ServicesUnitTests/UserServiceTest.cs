using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services.Implementations;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UnitTests.ServicesUnitTests
{
    [TestFixture]
    internal class UserServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IMapper> _mapperMock;
        private IUserService _userService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();

            _userService = new UserService(_userManagerMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task UpdateAsync_ValidParameters_UpdatesUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.UpdateAsync(userModel);

            // Assert
            _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));

            // Act
            var updatingUser = _userService.Invoking(s => s.UpdateAsync(userModel));

            // Assert
            await updatingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task UpdateAsync_EmailIsAlreadyTaken_ThrowsValidationException()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userToUpdate = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "newMail@gmail.com" };

            var newUserWithExistingEmail = new User { Id = "22fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "newMail@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByEmailAsync(userToUpdate.Email)).ReturnsAsync(newUserWithExistingEmail);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var updatingUser = _userService.Invoking(s => s.UpdateAsync(userToUpdate));

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
            await _userService.ChangePasswordAsync(user.Id, currentPassword, newPassword);

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
            var changingPassword = _userService.Invoking(s => s.ChangePasswordAsync(user.Id, currentPassword, newPassword));

            // Assert
            await changingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Not valid current password.");
        }

        [Test]
        public async Task ChangePasswordAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";
            var currentPassword = "password";
            var newPassword = "newpassword";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var changingPassword = _userService.Invoking(s => s.ChangePasswordAsync(id, currentPassword, newPassword));

            // Assert
            await changingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task FindByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserModel>(user)).Returns(userModel);

            // Act
            var actualUser = await _userService.FindByEmailAsync(user.Email);

            // Assert
            actualUser.Should().BeEquivalentTo(userModel);
        }

        [Test]
        public async Task FindByEmailAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var email = "test@gmail.com";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(default(User));

            // Act
            var gettingUser = _userService.Invoking(s => s.FindByEmailAsync(email));

            // Assert
            await gettingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User with such email does not exists.");
        }

        [Test]
        public async Task FindByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<UserModel>(user)).Returns(userModel);

            // Act
            var actualUser = await _userService.FindByIdAsync(user.Id);

            // Assert
            actualUser.Should().BeEquivalentTo(userModel);
        }

        [Test]
        public async Task FindByIdAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var gettingUser = _userService.Invoking(s => s.FindByIdAsync(id));

            // Assert
            await gettingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task CreateAsync_ValidUser_CreatesUser()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(x => x.Map<User>(userModel)).Returns(user);

            // Act
            await _userService.CreateAsync(userModel, password);

            // Assert
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), password), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ValidUser_AddsUserToUserRole()
        {
            // Arrange
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";
            var userRole = "User";

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(x => x.Map<User>(userModel)).Returns(user);

            // Act
            await _userService.CreateAsync(userModel, password);

            // Assert
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), userRole), Times.Once);
        }

        [Test]
        public async Task CreateAsync_UserAlreadyExists_ThrowsValidationException()
        {
            // Arrange
            var existingUser = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var userModel = new UserModel { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "user1@gmail.com" };
            var password = "password";

            _userManagerMock.Setup(x => x.FindByIdAsync(existingUser.Id)).ReturnsAsync(default(User));
            _userManagerMock.Setup(x => x.FindByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed());

            // Act
            var creatingUser = _userService.Invoking(s => s.CreateAsync(userModel, password));

            // Assert
            await creatingUser
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User already exists.");
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
            var actualRoles = await _userService.GetRolesAsync(user.Id);

            // Assert
            actualRoles.Should().BeEquivalentTo(roles);
        }

        [Test]
        public async Task GetRolesAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var id = "13fd42af-6a64-4022-bed4-8c7507cb67b9";

            _userManagerMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(default(User));

            // Act
            var gettingRoles = _userService.Invoking(s => s.GetRolesAsync(id));

            // Assert
            await gettingRoles
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User was not found.");
        }

        [Test]
        public async Task CheckPasswordAsync_UserNotFound_ThrowsValidationException()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "password";

            _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(default(User));

            // Act
            var checkingPassword = _userService.Invoking(s => s.CheckPasswordAsync(email, password));

            // Assert
            await checkingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("User with such email does not exists.");
        }

        [Test]
        public async Task CheckPasswordAsync_InvalidPassword_ThrowsValidationException()
        {
            // Arrange
            var password = "password";
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "test@gmail.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(false);

            // Act
            var checkingPassword = _userService.Invoking(s => s.CheckPasswordAsync(user.Email, password));

            // Assert
            await checkingPassword
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Wrong password.");
        }

        [Test]
        public async Task CheckPasswordAsync_ValidParameters_ThrowsValidationException()
        {
            // Arrange
            var password = "password";
            var user = new User { Id = "13fd42af-6a64-4022-bed4-8c7507cb67b9", Email = "test@gmail.com" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);

            // Act
            var checkingPassword = _userService.Invoking(s => s.CheckPasswordAsync(user.Email, password));

            // Assert
            await checkingPassword
                .Should().NotThrowAsync();
        }
    }
}
