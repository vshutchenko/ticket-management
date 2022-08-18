using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Services.Interfaces
{
    public interface IIdentityService
    {
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        Task UpdateUserAsync(UserModel user);

        Task<UserModel> GetUserByIdAsync(string userId);

        Task<UserModel> GetUserByEmailAsync(string email);

        Task<IList<string>> GetRolesAsync(string userId);

        Task<bool> CheckPasswordAsync(string userId, string password);

        Task<UserModel> AuthenticateAsync(string email, string password);

        Task CreateUserAsync(UserModel userModel, string password);

        Task AssignRoleAsync(string userId, string role);

        Task SeedInitialDataAsync();
    }
}
