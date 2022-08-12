using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.Entities;
using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Services.Implementations
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public IdentityService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser == null)
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

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
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

        public async Task<UserModel> GetUserAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            var model = _mapper.Map<UserModel>(existingUser);

            return model;
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            var roles = await _userManager.GetRolesAsync(existingUser);

            return roles;
        }

        public async Task CreateUserAsync(UserModel userModel, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(userModel.Email);

            if (existingUser == null)
            {
                var user = _mapper.Map<User>(userModel);
                user.UserName = user.Email;
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    await _userManager.AddToRoleAsync(createdUser, "User");
                }
                else
                {
                    throw new ValidationException("Cannot create user.");
                }
            }
            else
            {
                throw new ValidationException("User already exists.");
            }
        }

        public async Task AssignRoleAsync(string userId, string role)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser is null)
            {
                throw new ValidationException("User was not found.");
            }

            var result = await _userManager.AddToRoleAsync(existingUser, role);

            if (!result.Succeeded)
            {
                throw new ValidationException("Role was not found.");
            }
        }

        public async Task<UserModel> AuthenticateAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is null)
            {
                throw new ValidationException("User with such email does not exists.");
            }

            var result = await _signInManager.PasswordSignInAsync(existingUser, password, false, false);

            if (!result.Succeeded)
            {
                throw new ValidationException("Wrong password.");
            }

            var model = _mapper.Map<UserModel>(existingUser);

            return model;
        }

        public async Task SeedInitialDataAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole("Admin"),
                new IdentityRole("Venue manager"),
                new IdentityRole("User"),
                new IdentityRole("Event manager"),
            };

            foreach (var r in roles)
            {
                var existingRole = await _roleManager.FindByNameAsync(r.Name);

                if (existingRole is null)
                {
                    await _roleManager.CreateAsync(r);
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            var userRoles = new List<(User user, string role)>
            {
                (new User
                {
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "ru-RU",
                    TimeZoneId = "Eastern Standard Time",
                }, "Admin"),

                (new User
                {
                    UserName = "venueManager",
                    NormalizedUserName = "VENUEMANAGER",
                    Email = "manager1@gmail.com",
                    NormalizedEmail = "MANAGER1@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "ru-RU",
                    TimeZoneId = "Eastern Standard Time",
                }, "Venue manager"),

                (new User
                {
                    UserName = "user1",
                    NormalizedUserName = "USER1",
                    Email = "user1@gmail.com",
                    NormalizedEmail = "USER1@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "ru-RU",
                    TimeZoneId = "Eastern Standard Time",
                }, "User"),

                (new User
                {
                    UserName = "eventManager",
                    NormalizedUserName = "EVENTMANAGER",
                    Email = "eventManager@gmail.com",
                    NormalizedEmail = "EVENTMANAGER@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "ru-RU",
                    TimeZoneId = "Eastern Standard Time",
                }, "Event manager"),
            };

            foreach (var x in userRoles)
            {
                var existingUser = await _userManager.FindByEmailAsync(x.user.Email);

                if (existingUser is null)
                {
                    await _userManager.CreateAsync(x.user);
                    var createdUser = await _userManager.FindByEmailAsync(x.user.Email);
                    await _userManager.AddToRoleAsync(createdUser, x.role);
                }
            }
        }
    }
}
