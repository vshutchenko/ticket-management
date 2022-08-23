using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.IntegrationTests.Addition
{
    public class ContextSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                    Id = "39441f04-fb16-4e79-ae59-60bf8fac057d",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null!, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
                    TimeZoneId = "Eastern Standard Time",
                }, "Admin"),

                (new User
                {
                    Id = "bef9f5d7-d907-4ec2-a807-9abbdcf9e414",
                    UserName = "venueManager",
                    NormalizedUserName = "VENUEMANAGER",
                    Email = "manager1@gmail.com",
                    NormalizedEmail = "MANAGER1@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null!, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
                    TimeZoneId = "Eastern Standard Time",
                }, "Venue manager"),

                (new User
                {
                    Id = "ae6af83f-d680-4a71-9af5-6ec65c06f5b6",
                    UserName = "user1",
                    NormalizedUserName = "USER1",
                    Email = "user1@gmail.com",
                    NormalizedEmail = "USER1@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null!, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
                    TimeZoneId = "Eastern Standard Time",
                }, "User"),

                (new User
                {
                    Id = "d33655d7-af47-49c7-a004-64969e5b651f",
                    UserName = "eventManager",
                    NormalizedUserName = "EVENTMANAGER",
                    Email = "eventManager@gmail.com",
                    NormalizedEmail = "EVENTMANAGER@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = _userManager.PasswordHasher.HashPassword(null!, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
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
