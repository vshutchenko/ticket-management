using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Services.Implementations
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
