using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IIdentityService
    {
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task UpdateUserAsync(UserModel user);

        Task<UserModel> GetUserAsync(string userId);

        Task<IList<string>> GetRolesAsync(string userId);

        Task<UserModel> AuthenticateAsync(string email, string password);

        Task CreateUserAsync(UserModel userModel, string password);

        Task AssignRoleAsync(string userId, string role);

        Task SeedInitialDataAsync();
    }
}
