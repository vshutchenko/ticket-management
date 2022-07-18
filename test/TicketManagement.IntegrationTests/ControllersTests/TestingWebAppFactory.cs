using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.EntityFrameworkImplementations;
using TicketManagement.IntegrationTests.ControllersTests.Addition;

namespace TicketManagement.IntegrationTests.ControllersTests
{
    internal class TestingWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(async services =>
            {
                var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TicketManagementContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var testDb = new TestDatabase.TestDatabase();

                services.AddDbContext<TicketManagementContext>(options =>
                {
                    options.UseSqlServer(testDb.ConnectionString);
                });

                services.AddAntiforgery(t =>
                {
                    t.Cookie.Name = AntiForgeryTokenExtractor.Cookie;
                    t.FormFieldName = AntiForgeryTokenExtractor.Field;
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
            });
        }

        private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole("User"),
                new IdentityRole("Event manager"),
            };

            foreach (var r in roles)
            {
                var existingRole = await roleManager.FindByNameAsync(r.Name);

                if (existingRole is null)
                {
                    await roleManager.CreateAsync(r);
                }
            }
        }

        private async Task SeedUsersAsync(UserManager<User> userManager)
        {
            var userRoles = new List<(User user, string role)>
            {
                (new User
                {
                    Id = "d33655d7-af47-49c7-a004-64969e5b651f",
                    UserName = "testUser",
                    NormalizedUserName = "TESTUSER",
                    Email = "testUser@gmail.com",
                    NormalizedEmail = "TESTUSER@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
                    TimeZoneId = "Eastern Standard Time",
                    Balance = 100,
                }, "User"),

                (new User
                {
                    Id = "ae6af83f-d680-4a71-9af5-6ec65c06f5b6",
                    UserName = "eventManager",
                    NormalizedUserName = "EVENTMANAGER",
                    Email = "eventManager@gmail.com",
                    NormalizedEmail = "EVENTMANAGER@GMAIL.COM",
                    EmailConfirmed = false,
                    PasswordHash = userManager.PasswordHasher.HashPassword(null, "Password123#"),
                    SecurityStamp = string.Empty,
                    FirstName = "John",
                    LastName = "Doe",
                    CultureName = "en-US",
                    TimeZoneId = "Eastern Standard Time",
                }, "Event manager"),
            };

            foreach (var x in userRoles)
            {
                var existingUser = await userManager.FindByEmailAsync(x.user.Email);

                if (existingUser is null)
                {
                    await userManager.CreateAsync(x.user);
                    var createdUser = await userManager.FindByEmailAsync(x.user.Email);
                    await userManager.AddToRoleAsync(createdUser, x.role);
                }
            }
        }
    }
}
