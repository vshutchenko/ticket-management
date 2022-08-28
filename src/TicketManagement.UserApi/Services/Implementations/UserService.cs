using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketManagement.Core.Models;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.UserApi.Models;
using TicketManagement.UserApi.Services.Interfaces;

namespace TicketManagement.UserApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task ChangePasswordAsync(string id, string currentPassword, string newPassword)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser is null)
            {
                throw new ValidationException("User was not found.");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(existingUser, currentPassword);

            if (!isValidPassword)
            {
                throw new ValidationException("Not valid current password.");
            }

            var result = await _userManager.ChangePasswordAsync(existingUser, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                throw new ValidationException("Cannot change password.");
            }
        }

        public async Task CheckPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                throw new ValidationException("User with such email does not exists.");
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!isValidPassword)
            {
                throw new ValidationException("Wrong password.");
            }
        }

        public async Task CreateAsync(UserModel user, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                throw new ValidationException("User already exists.");
            }

            var newUser = new User
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CultureName = user.CultureName,
                TimeZoneId = user.TimeZoneId,
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                throw new ValidationException("Cannot create user.");
            }

            await _userManager.AddToRoleAsync(newUser, Roles.User);
        }

        public async Task<UserModel> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                throw new ValidationException("User with such email does not exists.");
            }

            var userModel = _mapper.Map<UserModel>(user);

            return userModel;
        }

        public async Task<UserModel> FindByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                throw new ValidationException("User was not found.");
            }

            var userModel = _mapper.Map<UserModel>(user);

            return userModel;
        }

        public async Task<IList<string>> GetRolesAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                throw new ValidationException("User was not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return roles;
        }

        public async Task UpdateAsync(UserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser is null)
            {
                throw new ValidationException("User was not found.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(user.Email);

            if (userWithSameEmail != null && userWithSameEmail.Id != existingUser.Id)
            {
                throw new ValidationException("This email is already taken.");
            }

            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.TimeZoneId = user.TimeZoneId;
            existingUser.CultureName = user.CultureName;
            existingUser.Balance = user.Balance;

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                throw new ValidationException("Cannot update user.");
            }
        }
    }
}
