using RestEase;
using TicketManagement.WebApplication.Clients.UserApi.Models;

namespace TicketManagement.WebApplication.Clients.UserApi
{
    [Header("Content-Type", "application/json")]
    public interface IUserClient
    {
        [Post("users/register")]
        public Task<string> RegisterAsync([Body] RegisterModel registerModel,
            CancellationToken cancellationToken = default);

        [Post("users/login")]
        public Task<string> LoginAsync([Body] LoginModel loginModel,
            CancellationToken cancellationToken = default);

        [Put("users")]
        public Task<string> UpdateAsync([Body] UserModel user,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Put("users/{id}/password")]
        public Task<string> ChangePassword([Path] string id,
            [Body] PasswordModel passwordModel,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("users/{id}")]
        public Task<UserModel> GetByIdAsync([Path] string id,
            [Header("Authorization")] string jwtToken,
            CancellationToken cancellationToken = default);

        [Get("users/validate")]
        public Task ValidateTokenAsync([Query] string token,
            CancellationToken cancellationToken = default);
    }
}
