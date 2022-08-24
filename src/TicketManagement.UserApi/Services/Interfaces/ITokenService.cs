using TicketManagement.UserApi.Models;

namespace TicketManagement.UserApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GetToken(UserModel user, IList<string> roles);
        bool ValidateToken(string token);
    }
}
