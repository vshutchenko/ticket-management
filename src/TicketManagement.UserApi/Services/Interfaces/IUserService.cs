using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> FindByEmailAsync(string email);

        Task<UserModel> FindByIdAsync(string id);

        Task CheckPasswordAsync(string email, string password);

        Task CreateAsync(UserModel user, string password);

        Task UpdateAsync(UserModel user);

        Task<IList<string>> GetRolesAsync(string id);

        Task ChangePasswordAsync(string id, string currentPassword, string newPassword);
    }
}
