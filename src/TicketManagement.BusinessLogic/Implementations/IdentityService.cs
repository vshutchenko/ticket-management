using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Implementations
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
            User existingUser = await _userManager.FindByIdAsync(user.Id);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            User userWithSameEmail = await _userManager.FindByEmailAsync(user.Email);

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

            IdentityResult result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                throw new ValidationException("Cannot update user.");
            }
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            User existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            bool isValidPassword = await _userManager.CheckPasswordAsync(existingUser, currentPassword);

            if (!isValidPassword)
            {
                throw new ValidationException("Not valid current password.");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(existingUser, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                throw new ValidationException("Cannot change password.");
            }
        }

        public async Task<UserModel> GetUserAsync(string userId)
        {
            User existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            UserModel model = _mapper.Map<UserModel>(existingUser);

            return model;
        }

        public async Task<IList<string>> GetRolesAsync(string userId)
        {
            User existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new ValidationException("User was not found.");
            }

            IList<string> roles = await _userManager.GetRolesAsync(existingUser);

            return roles;
        }

        public async Task CreateUserAsync(UserModel userModel, string password)
        {
            User existingUser = await _userManager.FindByEmailAsync(userModel.Email);

            if (existingUser == null)
            {
                User user = _mapper.Map<User>(userModel);
                user.UserName = user.Email;
                IdentityResult result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    User createdUser = await _userManager.FindByEmailAsync(user.Email);
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
            User existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser is null)
            {
                throw new ValidationException("User was not found.");
            }

            IdentityResult result = await _userManager.AddToRoleAsync(existingUser, role);

            if (!result.Succeeded)
            {
                throw new ValidationException("Role was not found.");
            }
        }

        public async Task<UserModel> AuthenticateAsync(string email, string password)
        {
            User existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser is null)
            {
                throw new ValidationException("User with such email does not exists.");
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(existingUser, password, false, false);

            if (!result.Succeeded)
            {
                throw new ValidationException("Wrong password.");
            }

            UserModel model = _mapper.Map<UserModel>(existingUser);

            return model;
        }

        public async Task SeedInitialDataAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole("Admin"),
                new IdentityRole("Venue manager"),
                new IdentityRole("User"),
                new IdentityRole("Event manager"),
            };

            foreach (IdentityRole r in roles)
            {
                IdentityRole existingRole = await _roleManager.FindByNameAsync(r.Name);

                if (existingRole is null)
                {
                    await _roleManager.CreateAsync(r);
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            List<(User user, string role)> userRoles = new List<(User user, string role)>
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

            foreach ((User user, string role) x in userRoles)
            {
                User existingUser = await _userManager.FindByEmailAsync(x.user.Email);

                if (existingUser is null)
                {
                    await _userManager.CreateAsync(x.user);
                    User createdUser = await _userManager.FindByEmailAsync(x.user.Email);
                    await _userManager.AddToRoleAsync(createdUser, x.role);
                }
            }
        }
    }
}
