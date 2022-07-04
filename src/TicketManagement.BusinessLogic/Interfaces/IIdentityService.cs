using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Models;

namespace TicketManagement.BusinessLogic.Interfaces
{
    public interface IIdentityService
    {
        Task ChangePassword(string userId, string currentPassword, string newPassword);
        Task UpdateUser(UserModel user);
        Task<UserModel> GetUserAsync(string userId);
        Task<IList<string>> GetRolesAsync(string userId);
        Task<UserModel> AuthenticateAsync(string email, string password);
        Task CreateAsync(UserModel userModel, string password);
        Task AssignRole(string userId, string role);
        Task SeedInitialData();
    }
}
